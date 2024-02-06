using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Accretion.GraphicHelpers;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Accretion.GameplayElements.Objects
{
    class RoundObject : SpaceObject
    {
        protected static Texture2D texture;
        private static bool hasLightSource = true;
        private static Random rand = new Random();
        protected static Vector2 textureCenter;

        public RoundObject(Vector2 location, Vector2 velocity, int mass, int radius)
            : base(location, velocity, mass, radius)
        {
        }

        public RoundObject(Vector2 location, Vector2 velocity, int mass, double density)
            : base(location, velocity, mass, density)
        {
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 cameraFieldLocation, float zoomLevel, Vector2? lightSource, int windowWidth, int windowHeight)
        {
            if (lightSource.HasValue)
            {
                if (texture == null || !hasLightSource)
                {
                    hasLightSource = true;
                    texture = AccretionGame.staticContent.Load<Texture2D>("sphere200");
                    textureCenter = new Vector2(texture.Width, texture.Height) / 2;
                }
            }
            else
            {
                if (texture == null || hasLightSource)
                {
                    hasLightSource = false;
                    texture = AccretionGame.staticContent.Load<Texture2D>("circle200");
                    textureCenter = new Vector2(texture.Width, texture.Height) / 2;
                }
            }

            float rotation = 0;
            if (lightSource.HasValue)
            {
                Vector2 direction = lightSource.Value - this.getFieldLocation();
                rotation = (float)Math.Atan2(direction.X, -direction.Y);
            }

            spriteBatch.Draw(texture, FieldAndScreenConversions.GetScreenLocation(this.getFieldLocation(), cameraFieldLocation, zoomLevel), null, color, rotation, textureCenter, (float)Math.Max((float)this.getRadius() / zoomLevel / 100, .01), SpriteEffects.None, 0);
        }

        

        public override List<SpaceObject> explode()
        {
            this.pendingRemoval = true;
            int ejectedFragments = rand.Next(3, 6);
            List<SpaceObject> fragments = new List<SpaceObject>(ejectedFragments);

            //decide how much mass each gets
            int totalWeight = 0;
            List<int> weights = new List<int>(ejectedFragments);
            for (int i = 0; i < ejectedFragments; i++)
            {
                int currentWeight = rand.Next(1, 10);
                totalWeight += currentWeight;
                weights.Add(currentWeight);
            }

            //create the new objects
            for (int i = 0; i < ejectedFragments; i++)
            {
                int fragmentMass = (int)Math.Floor((double)this.getMass() * weights[i] / totalWeight);
                if (fragmentMass > 0)
                {
                    SpaceObject fragment = new RoundObject(this.getFieldLocation(), this.getVelocity(), fragmentMass, this.getDensity());
                    bool fragmentLocationOverlapping = true;
                    int retries = 20;
                    while (fragmentLocationOverlapping && retries-- > 0)
                    {
                        double radians = 2 * Math.PI * rand.NextDouble();
                        double radius = (fragment.getRadius() + this.getRadius()) * 1.1;
                        Vector2 direction = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
                        Vector2 position = (float)radius * direction + this.getFieldLocation();
                        fragment.setFieldLocation(position);
                        fragment.addVelocity(direction * 8);
                        if (fragments.Count > 0)
                        {
                            bool allFragmentsClear = true;
                            foreach (SpaceObject existingFrag in fragments)
                            {
                                if ((existingFrag.getFieldLocation() - fragment.getFieldLocation()).LengthSquared() < Math.Pow(existingFrag.getRadius() + fragment.getRadius(), 2) * 1.5)
                                {
                                    allFragmentsClear = false;
                                    break;
                                }                                
                            }

                            if (allFragmentsClear)
                            {
                                fragmentLocationOverlapping = false;
                            }
                        }
                        else
                        {
                            fragmentLocationOverlapping = false;
                        }
                    }

                    if (retries == 0)
                    {
                        int j = 0; //awww shit how did we get here??
                        j++;
                    }

                    fragments.Add(fragment);
                }
            }

            return fragments;
        }
    }
}
