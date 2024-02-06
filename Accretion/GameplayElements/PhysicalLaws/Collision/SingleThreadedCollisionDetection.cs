using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;

#if WINDOWS
using System.Threading.Tasks;
#endif

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    public class SingleThreadedCollisionDetection : ICollisionDetection
    {
        private const int COLLISION_CHECK_SKIP_MAGIC_NUMBER = 300;
        private const int MAX_ALLOWED_COLLISION_CHECK_SKIPS = 15;
        private readonly Random rand = new Random();

        public bool collisionDetection(List<SpaceObject> collisionObjects)
        {
            bool collision = false;
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

                foreach (List<SpaceObject>[] partitionRow in partitionedObjects)
                {
                    foreach (List<SpaceObject> spaceObjects in partitionRow)
                    {
                        bool collisionInPartition = detectCollisionsInSinglePartition(spaceObjects);
                        if (collisionInPartition)
                        {
                            collision = true;
                        }
                    }
                }
            }

            return collision;
        }

        private bool detectCollisionsInSinglePartition(List<SpaceObject> spaceObjects)
        {
            //detect collisions
            //TODO: heavily optimize this so fewer comparisons are needed
            Boolean collision = false;
            //foreach (SpaceObject spaceObject1 in spaceObjects)
            for (int i = 0; i < spaceObjects.Count - 1; i++)
            {
                SpaceObject spaceObject1 = spaceObjects[i];
                if (spaceObject1 != null && !spaceObject1.pendingRemoval)
                {
                    //foreach (SpaceObject spaceObject2 in spaceObjects)
                    for (int j = i + 1; j < spaceObjects.Count; j++)
                    {
                        SpaceObject spaceObject2 = spaceObjects[j];
                        if (spaceObject2 != null && spaceObject1 != spaceObject2 && !spaceObject2.pendingRemoval)
                        {
                            if (spaceObject1.hasCollidedWith(spaceObject2))
                            {
                                //A collision has happend!
                                //TODO: handle the case where both are of similar size
                                //if (!spaceObject2.pendingRemoval && !spaceObject1.pendingRemoval)
                                //{
                                if (spaceObject1.getMass() >= spaceObject2.getMass())
                                {
                                    spaceObject1.absorbAndConserveMomentum(spaceObject2);
                                }
                                else
                                {
                                    spaceObject2.absorbAndConserveMomentum(spaceObject1);
                                }

                                collision = true;
                                //}
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
        }
    }
}