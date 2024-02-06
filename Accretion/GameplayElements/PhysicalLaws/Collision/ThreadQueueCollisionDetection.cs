using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    public class ThreadQueueCollisionDetection : ICollisionDetection
    {
        private const int COLLISION_CHECK_SKIP_MAGIC_NUMBER = 300;
        private const int MAX_ALLOWED_COLLISION_CHECK_SKIPS = 30;
        private readonly Random rand = new Random();

        private int threadCount;
        private List<Thread> threads;
        private List<AutoResetEvent> workerBlockers;
        private List<AutoResetEvent> mainThreadBlockers;

        //private Queue<List<SpaceObject>> partitionQueue = new Queue<List<SpaceObject>>();
        private Queue<List<SpaceObject>[]> partitionQueue = new Queue<List<SpaceObject>[]>();

        private readonly object queueLock = new object();

        public ThreadQueueCollisionDetection()
        {
 
            this.threadCount = Environment.ProcessorCount;
 
            threads = new List<Thread>(this.threadCount);
            workerBlockers = new List<AutoResetEvent>(this.threadCount);
            mainThreadBlockers = new List<AutoResetEvent>(this.threadCount);
            for (int i = 0; i < threadCount; i++)
            {
                workerBlockers.Add(new AutoResetEvent(false));
                mainThreadBlockers.Add(new AutoResetEvent(false));
                Thread thread = new Thread(concurrentCollisionWorker);
                threads.Add(thread);
                thread.Start(i);
            }
        }

        public bool collisionDetection(List<SpaceObject> collisionObjects)
        {
            if (collisionObjects != null && collisionObjects.Count > 0)
            {
                //draw a bounding rectangle around it
                Vector4 bound = createBoundingBox(collisionObjects);

                //decide how many partitions to make based on how many objects there are
                //TODO: what's an optimal number for this?
                int numPartitions = collisionObjects.Count / 2;
                int rootOfPartitions = (int)Math.Ceiling(Math.Sqrt(numPartitions));
                float partitionWidth = (bound.Y - bound.W) / rootOfPartitions;
                float partitionHeight = (bound.Z - bound.X) / rootOfPartitions;

                //build the partitions
                BoundingBox[,] partitions = new BoundingBox[rootOfPartitions, rootOfPartitions];
                List<SpaceObject>[][] partitionedObjects = new List<SpaceObject>[rootOfPartitions][];
                for (int i = 0; i < rootOfPartitions; i++)
                {
                    partitionedObjects[i] = new List<SpaceObject>[rootOfPartitions];
                    for (int j = 0; j < rootOfPartitions; j++)
                    {
                        float minX = i * partitionWidth + bound.W;
                        float minY = j * partitionHeight + bound.X;
                        float maxX = minX + partitionWidth;
                        float maxY = minY + partitionHeight;
                        BoundingBox subBound = new BoundingBox(new Vector3(minX, minY, 0), new Vector3(maxX, maxY, 0));
                        partitions[i, j] = subBound;
                        partitionedObjects[i][j] = new List<SpaceObject>();
                    }
                }

                //calculate what partition each object is in
                foreach (SpaceObject currentObject in collisionObjects)
                {
                    if (currentObject != null)
                    {
                        //find the home partition for an object, where its center is located
                        int xPartitionIndex = (int)((currentObject.getFieldLocation().X - bound.W) / partitionWidth);
                        int yPartitionIndex = (int)((currentObject.getFieldLocation().Y - bound.X) / partitionHeight);

                        //now check if the object is in all partitions within one object radius
                        //TODO: check if this math.ceiling is the right choice below, and starting with 0 or 1
                        int objectRadiusInPartitionWidth = 1;
                        int objectRadiusInPartitionHeight = 1;

                        if (currentObject.getRadius() > partitionWidth)
                        {
                            objectRadiusInPartitionWidth = (int)Math.Ceiling(currentObject.getRadius() / partitionWidth);
                        }

                        if (currentObject.getRadius() > partitionHeight)
                        {
                            objectRadiusInPartitionHeight = (int)Math.Ceiling(currentObject.getRadius() / partitionHeight);
                        }

                        for (int i = xPartitionIndex - objectRadiusInPartitionWidth; i <= xPartitionIndex + objectRadiusInPartitionWidth; i++)
                        {
                            for (int j = yPartitionIndex - objectRadiusInPartitionHeight; j <= yPartitionIndex + objectRadiusInPartitionHeight; j++)
                            {
                                if (i >= 0 && i < rootOfPartitions && j >= 0 && j < rootOfPartitions)
                                {
                                    BoundingBox boundingBox = partitions[i, j];

                                    if (boundingBox.Intersects(currentObject.boundingSphere))
                                    {
                                        partitionedObjects[i][j].Add(currentObject);
                                    }
                                }
                            }
                        }
                    }
                }

                //Work queue version per partion
                //partitionQueue = new Queue<List<SpaceObject>>(partitionedObjects);
                //foreach (List<SpaceObject>[] partitionRow in partitionedObjects)
                //{
                //    foreach (List<SpaceObject> partition in partitionRow)
                //    {
                //        partitionQueue.Enqueue(partition);
                //    }
                //}

                //work queue per row
                foreach (List<SpaceObject>[] partitionRow in partitionedObjects)
                {
                    partitionQueue.Enqueue(partitionRow);
                }

                foreach (AutoResetEvent autoResetEvent in workerBlockers)
                {
                    autoResetEvent.Set();
                }

                foreach (AutoResetEvent autoResetEvent in mainThreadBlockers)
                {
                    autoResetEvent.WaitOne();
                }
            }

            return true;
        }

        //Work queue version
        private void concurrentCollisionWorker(object autoResetEventNumber)
        {
 

            int threadNumber = (int)autoResetEventNumber;
            while (true)
            {
                List<SpaceObject>[] partitionRow = null;
                lock (queueLock)
                {
                    if (partitionQueue.Count > 0)
                    {
                        partitionRow = partitionQueue.Dequeue();
                        if (partitionRow == null)
                        {
                            mainThreadBlockers[threadNumber].Set();
                            return;
                        }
                    }
                }

                if (partitionRow != null)
                {
                    foreach (List<SpaceObject> partition in partitionRow)
                    {
                        detectCollisions(partition);
                    }
                }
                else
                {
                    mainThreadBlockers[threadNumber].Set();
                    workerBlockers[threadNumber].WaitOne();
                }
            }
        }

        private bool detectCollisions(List<SpaceObject> spaceObjects)
        {
            //detect collisions
            //TODO: heavily optimize this so fewer comparisons are needed
            Boolean collision = false;
            foreach (SpaceObject spaceObject1 in spaceObjects)
            {
                if (spaceObject1 != null && !spaceObject1.pendingRemoval)
                {
                    int allowedCollisionCheckSkips;
                    if (spaceObject1 is PlayerObject)
                    {
                        allowedCollisionCheckSkips = 0;
                    }
                    else
                    {
                        allowedCollisionCheckSkips = Math.Min((int)Math.Ceiling((COLLISION_CHECK_SKIP_MAGIC_NUMBER / (spaceObject1.getVelocity().Length() + 1))), MAX_ALLOWED_COLLISION_CHECK_SKIPS);
                    }

                    if (spaceObject1.skippedCollisionChecks < allowedCollisionCheckSkips)
                    {
                        spaceObject1.skippedCollisionChecks++;
                    }
                    else
                    {
                        spaceObject1.skippedCollisionChecks = rand.Next(-2, 2); //I used to set this to 0, but I think this will spread the checks out more
                        foreach (SpaceObject spaceObject2 in spaceObjects)
                        {
                            if (spaceObject2 != null && spaceObject1 != spaceObject2 && !spaceObject2.pendingRemoval)
                            {
                                if (spaceObject1.hasCollidedWith(spaceObject2))
                                {
                                    //A collision has happend!
                                    //TODO: handle the case where both are of similar size
                                    if (!spaceObject2.pendingRemoval && !spaceObject1.pendingRemoval)
                                    {
                                        if (spaceObject1.getMass() >= spaceObject2.getMass())
                                        {
                                            spaceObject1.absorbAndConserveMomentum(spaceObject2);
                                        }
                                        else
                                        {
                                            spaceObject2.absorbAndConserveMomentum(spaceObject1);
                                        }

                                        collision = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return collision;
        }

        private Vector4 createBoundingBox(List<SpaceObject> spaceObjects)
        {
            Vector2 firstLocation = spaceObjects.First().getFieldLocation();

            //using a vector4 as a hacky rectangle here, since it supports Float. w=xMin, x=yMin, y=xMax, z=yMax. Confusuing, I know.
            Vector4 bound = new Vector4(firstLocation.X, firstLocation.Y, firstLocation.X, firstLocation.Y);
            //Object[] locks = { new object(), new object(), new object(), new object() };

            foreach (SpaceObject spaceObject in spaceObjects)
            {
                //min x
                if (spaceObject != null)
                {
                    if (spaceObject.getFieldLocation().X < bound.W)
                    {
                        if (spaceObject.getFieldLocation().X < bound.W)
                        {
                            bound.W = spaceObject.getFieldLocation().X;
                        }
                    }
                    //max x
                    else if (spaceObject.getFieldLocation().X > bound.Y)
                    {
                        if (spaceObject.getFieldLocation().X > bound.Y)
                        {
                            bound.Y = spaceObject.getFieldLocation().X;
                        }
                    }

                    if (spaceObject.getFieldLocation().Y < bound.X)
                    {
                        if (spaceObject.getFieldLocation().Y < bound.X)
                        {
                            bound.X = spaceObject.getFieldLocation().Y;
                        }
                    }
                    else if (spaceObject.getFieldLocation().Y > bound.Z)
                    {
                        if (spaceObject.getFieldLocation().Y > bound.Z)
                        {
                            bound.Z = spaceObject.getFieldLocation().Y;
                        }
                    }
                }
            }

            return bound;
        }

        public void Dispose()
        {
            foreach (AutoResetEvent autoResetEvent in workerBlockers)
            {
                lock (queueLock)
                {
                    partitionQueue.Enqueue(null);
                }

                autoResetEvent.Set();
            }

            foreach (AutoResetEvent autoResetEvent in mainThreadBlockers)
            {
                autoResetEvent.Set();
            }

            foreach (Thread thread in threads)
            {
                //thread.Abort();
                thread.Join();
            }

            foreach (AutoResetEvent autoResetEvent in workerBlockers)
            {
                autoResetEvent.Close();
            }

            foreach (AutoResetEvent autoResetEvent in mainThreadBlockers)
            {
                autoResetEvent.Close();
            }
        }
    }
}