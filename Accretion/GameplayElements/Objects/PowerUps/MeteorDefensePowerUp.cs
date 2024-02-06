using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Accretion.GameplayObjects;
using Microsoft.Xna.Framework.Graphics;
using Accretion.GraphicHelpers;
using Microsoft.Xna.Framework.Audio;

namespace Accretion.GameplayElements.Objects.PowerUps
{
    class MeteorDefensePowerUp : PowerUp
    {
        protected double rangeFactor = 20; //int radii
        protected readonly TimeSpan animationlength = TimeSpan.FromSeconds(0.5);
        protected Vector2? lastHitDirection;

        private static SoundEffect soundEffect;
        private static SoundEffect noTarget;

        protected static char displayChar;

        static MeteorDefensePowerUp()
        {
            char.TryParse("M", out displayChar);
            soundEffect = AccretionGame.staticContent.Load<SoundEffect>("39459__the-bizniss__laser");
            noTarget = AccretionGame.staticContent.Load<SoundEffect>("2014__e-p-manchester__flash");
        }

        public override char getFieldDisplayCharacter()
        {
            return displayChar;
        }

        public MeteorDefensePowerUp(int uses, double rangeFactor) : base(uses) {
           char.TryParse("M", out displayChar);
           this.rangeFactor = rangeFactor;
           if (soundEffect == null || noTarget == null)
           {
               soundEffect = AccretionGame.staticContent.Load<SoundEffect>("39459__the-bizniss__laser");
               noTarget = AccretionGame.staticContent.Load<SoundEffect>("2014__e-p-manchester__flash");
           }
        }

        public MeteorDefensePowerUp(int uses, double rangeFactor, Vector2 location, Vector2 velocity)
            : base(uses, location, velocity)
        {
            this.rangeFactor = rangeFactor;
            char.TryParse("M", out displayChar);
            if (soundEffect == null || noTarget == null)
            {
                soundEffect = AccretionGame.staticContent.Load<SoundEffect>("39459__the-bizniss__laser");
                noTarget = AccretionGame.staticContent.Load<SoundEffect>("2014__e-p-manchester__flash");
            }
        }

        public override void use(PlayerObject player, ref Field field)
        {
            if (uses > 0 && player != null && field != null && player.pendingRemoval == false && player.getMass() > 0)
            {
                //find the closest non-player object that is within the targetable distance and mass range
                // TODO: should parallelize
                double closestDistance = Int32.MaxValue;
                SpaceObject closestObject = null;
                Random rand = new Random();

                foreach (SpaceObject spaceObject in field.getSpaceObjects())
                {
                    if (spaceObject != null)
                    {
                        double distance = Math.Abs((spaceObject.getFieldLocation() - player.getFieldLocation()).Length());
                        if (distance < rangeFactor * player.getRadius()
                            && distance < closestDistance 
                            && spaceObject.getMass() > player.getMass() * 4 / 5 //don't explode small things
                            && spaceObject.getMass() < player.getMass() * 20 //don't explode huge things                       
                            && spaceObject != player //don't let them explode themself
                            && !spaceObject.hasGravity) //don't let them explode the sun
                        {
                            closestDistance = distance;
                            closestObject = spaceObject;                            
                        }
                    }
                }

                //destory it and make some smaller objects with that mass
                if (closestObject != null)
                {
                    lastHitDirection = player.getFieldLocation() - closestObject.getFieldLocation();
                    base.use(player, ref field);
                    List<SpaceObject> fragments = closestObject.explode();
                    field.removeSpaceObject(closestObject);
                    field.addSpaceObjects(fragments);
                }
                else
                {
                    noTarget.Play();
                }
            }
        }

        private bool isInUse()
        {
            return lastUsed != null && DateTime.UtcNow < lastUsed + animationlength;
        }

        public override void drawAttached(SpriteBatch spriteBatch, Vector2 cameraFieldLocation, float zoomLevel, Vector2? lightSource, PlayerObject player, int windowWidth, int windowHeight)
        {
            if (this.isInUse() && lastHitDirection.HasValue)
            {
                Vector2 playerScreenLocation = FieldAndScreenConversions.GetScreenLocation(player.getFieldLocation(), cameraFieldLocation, zoomLevel);
                Texture2D helperLineTexture;
                helperLineTexture = LineCreator.CreateLine((int)(player.getRadius() / zoomLevel / 8), (int)(lastHitDirection.Value.Length() / zoomLevel), Color.Purple, spriteBatch.GraphicsDevice);
                LineCreator.DrawLine(helperLineTexture, playerScreenLocation, lastHitDirection.Value, Color.Red, spriteBatch);
            }

            base.drawAttached(spriteBatch, cameraFieldLocation, zoomLevel, lightSource, player, windowWidth, windowHeight);
        }

        public override SoundEffect getSoundEffect()
        {
            return soundEffect;
        }

        public override string getPowerName()
        {
            return "Meteor Defense Laser";
        }

        public override bool onUpdate(PlayerObject player, Field field)
        {
            return this.isInUse();
        }
    }
}
