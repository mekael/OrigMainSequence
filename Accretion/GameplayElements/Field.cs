using Accretion.GameplayElements.Objects;
using Accretion.GameplayElements.PhysicalLaws.Collision;
using Accretion.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;

namespace Accretion.GameplayObjects
{
    internal class Field : IDisposable
    {
        public SpaceObject cameraLockedObject;
        public float initialZoomLevel;
        public float startingZoomLevel;
        public float zoomLevel; //the game will transition gradually from initialZoomLevel to zoomLevel when play begins.
        public Vector2 mapSize;
        public IGravitationalLaw gravity;

        private List<SpaceObject> collisionObjects = new List<SpaceObject>();
        private List<SpaceObject> gravitationalObjects = new List<SpaceObject>();
        private PlayerObject player;
        private Rectangle mapLimits;
        private readonly Vector2 DEFAULT_CAMERA_LOCATION = Vector2.Zero;
        private Vector2 cameraWindowCenter;
        private DateTime? levelStart = null;
        private static readonly TimeSpan initialZoomInInterval = TimeSpan.FromSeconds(2);
        private MouseHelper mouseHandler = new MouseHelper();
        private bool doneIntroZooming = false; //a flag to note when we are done with level-start zoom in effect
        private float? lastAutoZoom;

#if WINDOWS
        public ICollisionDetection collisionDetection = new TPLCollisionDetection();
#elif XBOX
        public ICollisionDetection collisionDetection = new SingleThreadedCollisionDetection();
#elif WINDOWS_PHONE
        public ICollisionDetection collisionDetection = new SingleThreadedCollisionDetection();
#endif

        //level conditionals
        public bool wrapEdges = false;

        public Field()
        {
            cameraWindowCenter = DEFAULT_CAMERA_LOCATION;
        }

        public void addSpaceObject(SpaceObject spaceObject)
        {
            this.collisionObjects.Add(spaceObject);
        }

        public void addSpaceObjects(List<SpaceObject> spaceObjects)
        {
            this.collisionObjects.AddRange(spaceObjects);
        }

        public bool removeSpaceObject(SpaceObject spaceObject)
        {
            return this.collisionObjects.Remove(spaceObject);
        }

        public ReadOnlyCollection<SpaceObject> getSpaceObjects()
        {
            return this.collisionObjects.AsReadOnly();
        }

        public ReadOnlyCollection<SpaceObject> getGravitationalObjects()
        {
            return this.gravitationalObjects.AsReadOnly();
        }

        public void addGravitationalObjects(List<SpaceObject> gravitationalObjects)
        {
            foreach (SpaceObject newGravitatingObject in gravitationalObjects)
            {
                if (!this.collisionObjects.Contains(newGravitatingObject))
                {
                    this.collisionObjects.Add(newGravitatingObject);
                }

                if (!this.gravitationalObjects.Contains(newGravitatingObject))
                {
                    this.gravitationalObjects.Add(newGravitatingObject);
                    newGravitatingObject.hasGravity = true;
                }
            }
        }

        public void removeGravitationalObject(SpaceObject gravitationalObject, bool butDontRemoveFromField)
        {
            this.gravitationalObjects.Remove(gravitationalObject);
            gravitationalObject.hasGravity = false;

            if (!butDontRemoveFromField)
            {
                gravitationalObject.pendingRemoval = true;
            }
        }

        public void setPlayer(PlayerObject player)
        {
            this.collisionObjects.Add(player);
            this.player = player;
        }

        public PlayerObject getPlayer()
        {
            return this.player;
        }

        public void setMapSize(Vector2 mapDimensions)
        {
            this.mapSize = mapDimensions;
            this.mapLimits = new Rectangle((int)(mapSize.X / 2 * -1), (int)(mapSize.Y / 2 * -1), (int)mapSize.X, (int)mapSize.Y);
        }

        public void update()
        {
            bool cleanupOffMapObjects = false;

            if (!this.doneIntroZooming)
            {
                if (!this.levelStart.HasValue)
                {
                    this.levelStart = DateTime.UtcNow;
                    this.lastAutoZoom = this.zoomLevel;
                }
                else if (DateTime.UtcNow - this.levelStart <= initialZoomInInterval && this.zoomLevel == this.lastAutoZoom)
                {
                    this.zoomLevel = (float)(this.initialZoomLevel + (this.startingZoomLevel - this.initialZoomLevel) * (DateTime.UtcNow - this.levelStart).Value.TotalMilliseconds / initialZoomInInterval.TotalMilliseconds);
                    this.lastAutoZoom = this.zoomLevel;
                }
                else
                {
                    this.doneIntroZooming = true;
                }
            }

            //Vector2 offset;
            //if (this.player != null)
            //{
            //    offset = this.player.getFieldLocation();
            //}
            //else if (this.cameraWindowCenter != null)
            //{
            //    offset = this.cameraWindowCenter;
            //}
            //else
            //{
            //    offset = DEFAULT_CAMERA_LOCATION;
            //}

            foreach (SpaceObject currentObject in collisionObjects)
            //Parallel.ForEach(collisionObjects, currentObject =>
            {
                // update the locations based on velocity
                if (currentObject != null)
                {
                    currentObject.setFieldLocation(currentObject.getFieldLocation() + currentObject.getVelocity());

                    if (wrapEdges)
                    {
                        //if anything went off the edge, move it to the other edge
                        if (currentObject.getFieldLocation().X < mapLimits.Left)
                        {
                            Vector2 fieldLocation = currentObject.getFieldLocation();
                            fieldLocation.X = mapLimits.Right;
                            currentObject.setFieldLocation(fieldLocation);
                        }
                        else if (currentObject.getFieldLocation().X > mapLimits.Right)
                        {
                            Vector2 fieldLocation = currentObject.getFieldLocation();
                            fieldLocation.X = mapLimits.Left;
                            currentObject.setFieldLocation(fieldLocation);
                        }
                        else if (currentObject.getFieldLocation().Y < mapLimits.Top)
                        {
                            Vector2 fieldLocation = currentObject.getFieldLocation();
                            fieldLocation.Y = mapLimits.Bottom;
                            currentObject.setFieldLocation(fieldLocation);
                        }
                        else if (currentObject.getFieldLocation().Y > mapLimits.Bottom)
                        {
                            Vector2 fieldLocation = currentObject.getFieldLocation();
                            fieldLocation.Y = mapLimits.Top;
                            currentObject.setFieldLocation(fieldLocation);
                        }
                    }
                    else
                    {
                        //if anything went off the edge, kill it..
                        if (currentObject.getFieldLocation().X < mapLimits.Left ||
                            currentObject.getFieldLocation().X > mapLimits.Right ||
                            currentObject.getFieldLocation().Y < mapLimits.Top ||
                            currentObject.getFieldLocation().Y > mapLimits.Bottom)
                        {
                            currentObject.pendingRemoval = true;
                            currentObject.setMass(0);
                            cleanupOffMapObjects = true;
                        }
                    }
                }
                //});
            }

            bool collision = collisionDetection.collisionDetection(this.collisionObjects);

            //remove any absorbed or off-map objects
            if (collision || cleanupOffMapObjects)
            {
                //this could be more parallelized
                this.gravitationalObjects = this.gravitationalObjects.Where(spaceObject => spaceObject != null && !spaceObject.pendingRemoval && spaceObject.getMass() > 0).ToList();
                this.collisionObjects = this.collisionObjects.Where(spaceObject => spaceObject != null && !spaceObject.pendingRemoval && spaceObject.getMass() > 0).ToList();
            }

            //update the velocity based on gravitational forces
            //Parallel.ForEach(collisionObjects, currentObject =>
            foreach (SpaceObject currentObject in collisionObjects)
            {
                if (currentObject != null)
                {
                    foreach (SpaceObject gravitatingObject in gravitationalObjects)
                    {
                        if (gravitatingObject != null)
                        {
                            gravity.applyAcceleration(currentObject, gravitatingObject);
                        }
                    }
                }
                //});
            }

            if (player != null)
            {
                player.onUpdate(this);
            }
        }

        public Vector2 getScreenLocation(SpaceObject spaceObject)
        {
            return (spaceObject.getFieldLocation() - cameraWindowCenter) / zoomLevel;
        }

        public void draw(SpriteBatch spriteBatch, int windowWidth, int windowHeight)
        {
            if (cameraLockedObject != null && !float.IsNaN(cameraLockedObject.getFieldLocation().X) && !float.IsNaN(cameraLockedObject.getFieldLocation().Y))
            {
                cameraWindowCenter = this.cameraLockedObject.getFieldLocation() - new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) * zoomLevel / 2;
            }

            //if (player != null && player.getMass() > 0)
            //{
            //    mouseHandler.drawHelperVector(spriteBatch, player, cameraWindowCenter, zoomLevel);
            //}

            Dictionary<SpaceObject, Vector2?> lightingDirections = new Dictionary<SpaceObject, Vector2?>(collisionObjects.Count);
            //Parallel.ForEach(collisionObjects, currentObject =>
            foreach (SpaceObject currentObject in collisionObjects)
            {
                if (currentObject != null)
                {
                    Vector2? lightSource = findDistanceWeightedAverageLightSource(currentObject, gravitationalObjects);
                    //lock (lightingDirections)
                    {
                        lightingDirections.Add(currentObject, lightSource);
                    }
                }
                //});
            }

            foreach (SpaceObject currentObject in this.collisionObjects)
            {
                if (currentObject != null)
                {
                    currentObject.draw(spriteBatch, cameraWindowCenter, zoomLevel, lightingDirections[currentObject], windowWidth, windowHeight);
                }
            }
        }

        //TODO: this is a performance nightmare. wtf.
        private Vector2? findDistanceWeightedAverageLightSource(SpaceObject spaceObject, List<SpaceObject> lightSources)
        {
            if (lightSources == null || lightSources.Count < 1)
            {
                return null;
            }
            else
            {
                if (lightSources.Count == 1)
                {
                    return lightSources.First().getFieldLocation();
                }
                else
                {
                    Vector2 weightedSum = Vector2.Zero;
                    float weights = 0f;
                    //TODO: parallelize
                    foreach (SpaceObject lightSource in lightSources)
                    {
                        float distance = (spaceObject.getFieldLocation() - lightSource.getFieldLocation()).LengthSquared();
                        if (distance != 0 && !float.IsNaN(distance))
                        {
                            float weight = 1 / distance;
                            weights += weight;
                            weightedSum += (lightSource.getFieldLocation() * weight);
                        }
                    }

                    return weightedSum / weights;
                }
            }
        }

        public void Dispose()
        {
            if (this.collisionDetection != null)
            {
                this.collisionDetection.Dispose();
                this.collisionDetection = null;
            }
        }
    }
}