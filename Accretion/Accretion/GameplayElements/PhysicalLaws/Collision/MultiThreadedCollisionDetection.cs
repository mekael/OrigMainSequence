using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Accretion.GameplayElements.Objects;
using System.Threading;
using System.Collections;

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    public class MultiThreadedCollisionDetection : ICollisionDetection
    {
        private const int COLLISION_CHECK_SKIP_MAGIC_NUMBER = 300;
        private const int MAX_ALLOWED_COLLISION_CHECK_SKIPS = 30;
        private readonly Random rand = new Random();

        private int threadCount;
        private List<Thread> threads;
        private List<AutoResetEvent> workerBlockers;
        private List<AutoResetEvent> mainThreadBlockers;
        private List<SpaceObject>[][] partitionedSpaceObjects;
        private readonly object queueLock = new object();

        private bool working = true;

        public MultiThreadedCollisionDetection()
        {
#if WINDOWS
            this.threadCount = Environment.ProcessorCount;
#elif WINDOWS_PHONE
            this.threadCount = 1;
#elif XBOX
            this.threadCount = 4;
#endif
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
                BoundingBox[,] partitionBounds = new BoundingBox[rootOfPartitions, rootOfPartitions];
                partitionedSpaceObjects = new List<SpaceObject>[rootOfPartitions][];
                for (int i = 0; i < rootOfPartitions; i++)
                {
                    partitionedSpaceObjects[i] = new List<SpaceObject>[rootOfPartitions];
                    for (int j = 0; j < rootOfPartitions; j++)
                    {
                        float minX = i * partitionWidth + bound.W;
                        float minY = j * partitionHeight + bound.X;
                        float maxX = minX + partitionWidth;
                        float maxY = minY + partitionHeight;
                        BoundingBox subBound = new BoundingBox(new Vector3(minX, minY, 0), new Vector3(maxX, maxY, 0));
                        partitionBounds[i, j] = subBound;
                        partitionedSpaceObjects[i][j] = new List<SpaceObject>();
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
                                    BoundingBox boundingBox = partitionBounds[i, j];

                                    if (boundingBox.Intersects(currentObject.boundingSphere))
                                    {
                                        partitionedSpaceObjects[i][j].Add(currentObject);
                                    }
                                }
                            }
                        }
                    }
                }                

                //Fire off the workers. They will process the partitionedSpaceObjects 2d array populated above.
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
            int threadNum = (int)autoResetEventNumber;

#if XBOX
            if (threadNum == 0)
            {
                Thread.CurrentThread.SetProcessorAffinity(1);
            }
            else if (threadNum == 1)
            {
                Thread.CurrentThread.SetProcessorAffinity(3);
            }
            else if (threadNum == 2)
            {
                Thread.CurrentThread.SetProcessorAffinity(4);
            }
            else if (threadNum == 3)
            {
                Thread.CurrentThread.SetProcessorAffinity(5);
            }
#endif

            while (this.working)
            {
                workerBlockers[threadNum].WaitOne();
                if (this.partitionedSpaceObjects != null && this.partitionedSpaceObjects.Count() > 0)
                {
                    int partitionRowsPerThread = this.partitionedSpaceObjects.Count() / threadCount + 1;

                    int minPartitionRow = threadNum * partitionRowsPerThread;
                    int maxPartitionRow = minPartitionRow + partitionRowsPerThread;

                    if (maxPartitionRow > this.partitionedSpaceObjects.Count())
                    {
                        maxPartitionRow = this.partitionedSpaceObjects.Count();
                    }

                    for (int i = minPartitionRow; i < maxPartitionRow; i++)
                    {
                        foreach (List<SpaceObject> partition in this.partitionedSpaceObjects[i])
                        {
                            detectCollisions(partition);
                        }
                    }
                }

                mainThreadBlockers[threadNum].Set();
            }
        }

        private bool detectCollisions(List<SpaceObject> spaceObjects)
        {
            //detect collisions
            //TODO: heavily optimize this so fewer comparisons are needed
            Boolean collision = false;
            if (spaceObjects != null)
            {
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
            this.working = false;

            foreach (AutoResetEvent autoResetEvent in workerBlockers)
            {
                autoResetEvent.Set();
            }

            foreach (AutoResetEvent autoResetEvent in mainThreadBlockers)
            {
                autoResetEvent.Set();
            }

            foreach (Thread thread in threads)
            {
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
