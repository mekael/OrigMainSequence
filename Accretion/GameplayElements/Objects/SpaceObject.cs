using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Accretion.GraphicHelpers;
using Accretion.GameplayElements.Objects.PowerUps;

namespace Accretion.GameplayElements.Objects
{
    public abstract class SpaceObject
    {
        public Boolean unmoveable = false;
        public int ejectionSpeed;
        public BoundingSphere boundingSphere;

        private Vector2 fieldLocation;
        private Vector2 velocity;
        
        private int mass;
        private int radius;
        private double density;
        private double secretMassModifier = 1.0;

        protected Color color;
        protected SoundEffect absorptionSound;

        public bool pendingRemoval;
        public bool hasGravity = false;

        public int skippedCollisionChecks = Int32.MaxValue - 1;

        public SpaceObject(Vector2 location, Vector2 velocity, int mass, int radius)
        {
            this.setFieldLocation(location);
            this.velocity = velocity;
            this.mass = mass;
            this.radius = radius;
            this.boundingSphere.Radius = radius;
            if (radius != 0)
            {
                this.density = mass / Math.Pow(radius, 2);
            }
            else
            {
                this.density = 0;
            }

            this.updateColor();
        }

        public SpaceObject(Vector2 location, Vector2 velocity, int mass, double density) : 
            this(location, velocity,  mass, (int) Math.Ceiling(Math.Sqrt(mass / density)))
        {
        }

        public int getRadius()
        {
            return this.radius;
        }

        public Vector2 getFieldLocation()
        {
            return this.fieldLocation;
        }

        public void setFieldLocation(Vector2 fieldLocation)
        {
            if (float.IsNaN(fieldLocation.X) || float.IsNaN(fieldLocation.Y))
            {
                throw new ArgumentOutOfRangeException("Invalid position detected.");
            }

            this.fieldLocation = fieldLocation;
            this.boundingSphere.Center = new Vector3(fieldLocation, 0);
        }

        public virtual void addMass(int mass)
        {
            this.setMass(this.getBaseMass() + mass);
        }

        public virtual void setMass(int mass)
        {
            this.mass = mass;
            this.updateRadius();
            this.updateColor();
        }

        public int getMass()
        {
            return (int)Math.Ceiling(this.mass * secretMassModifier);
        }

        /// <summary>
        /// Returns the mass, uneffected by the secretMassModifier. This is useful for example
        /// when a mass is curring under the influence of a shrink ray but needs to absorb another
        /// object.
        /// </summary>
        /// <returns></returns>
        public int getBaseMass()
        {
            return this.mass;
        }

        public void setSecretMassModifier(double value)
        {
            this.secretMassModifier = value;
            if (this.secretMassModifier < 0.01)
            {
                this.secretMassModifier = 0.01;
            }
            else if (this.secretMassModifier > 1)
            {
                this.secretMassModifier = 1;
            }

            this.updateRadius();
        }

        public double getSecretMassModifier()
        {
            return this.secretMassModifier;
        }

        public virtual void setDensity(double density)
        {
            if (this.density != density)
            {
                this.density = density;
                this.updateRadius();
            }
        }

        public virtual void absorbAndConserveMomentum(SpaceObject otherObject)
        {
            if (!(otherObject is PowerUp))
            {
                lock (otherObject)
                {
                    int otherMass = otherObject.getMass();
                    this.addMomentum(otherMass, otherObject.getVelocity());
                    this.addMass(otherMass);
                    otherObject.setMass(0);
                    otherObject.pendingRemoval = true;
                    if (otherObject.hasGravity)
                    {
                        this.hasGravity = true;
                    }
                }

                if (absorptionSound != null)
                {
                    absorptionSound.Play();
                }
            }
        }

        public void addMomentum(int impactingMass, Vector2 impactingVelovity)
        {
            if (!this.unmoveable)
            {
                Vector2 myMomentum = this.velocity * this.getMass();
                Vector2 theirMomentum = impactingVelovity * impactingMass;
                int totalMass = this.getMass() + impactingMass;
                this.velocity = (myMomentum + theirMomentum) / totalMass;
            }
        }

        public double getDensity()
        {
            return this.density;
        }

        private void updateRadius()
        {
            this.radius = (int)Math.Ceiling(Math.Sqrt(this.getMass() / this.getDensity()));
            this.boundingSphere.Radius = this.radius;
        }

        private void updateColor()
        {
            this.color = BlackBodyRadiationHelper.chooseColor(this.getBaseMass());
        }

        public Vector2 getVelocity()
        {
            return this.velocity;
        }

        public void addVelocity(Vector2 additionalVelocity)
        {
            if (!this.unmoveable)
            {
                this.velocity += additionalVelocity;
            }
        }

        public void setVelocity(Vector2 velocity)
        {
            if (!this.unmoveable)
            {
                this.velocity = velocity;
            }
        }

        public bool hasCollidedWith(SpaceObject otherObject)
        {
            int collisionDistance = this.getRadius() + otherObject.getRadius();
            Vector2 separation = this.getFieldLocation() - otherObject.getFieldLocation();
            float distance = separation.Length();
            return distance < collisionDistance;
        }

        public float distanceFrom(SpaceObject otherSpaceObject)
        {
            return (this.getFieldLocation() - otherSpaceObject.getFieldLocation()).Length();
        }

        public abstract List<SpaceObject> explode();

        public abstract void draw(SpriteBatch spriteBatch, Vector2 cameraLocation, float zoomLevel, Vector2? lightSource, int windowWidth, int windowHeight);
    }
}
