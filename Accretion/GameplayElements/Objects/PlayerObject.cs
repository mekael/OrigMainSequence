using Accretion.AudioHelpers;
using Accretion.GameplayElements.Objects.PowerUps;
using Accretion.GameplayObjects;
using Accretion.GraphicHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Accretion.GameplayElements.Objects
{
    internal class PlayerObject : RoundObject
    {
        public readonly double baseDensity;
        public PowerUp powerUp;

        protected static SoundEffect ejectionSound;
        protected static SoundEffect failedEjectionSound;

#if !WINDOWS_PHONE
        private Texture2D helperLineTexture;
#endif

        public PlayerObject(Vector2 location, Vector2 velocity, int mass, int radius, int ejectionSpeed)
            : base(location, velocity, mass, radius)
        {
            this.ejectionSpeed = ejectionSpeed;
            this.baseDensity = this.getDensity();
            if (absorptionSound == null || ejectionSound == null || failedEjectionSound == null)
            {
                this.loadSounds();
            }
        }

        public PlayerObject(Vector2 location, Vector2 velocity, int mass, double density, int ejectionSpeed)
            : base(location, velocity, mass, density)
        {
            this.ejectionSpeed = ejectionSpeed;
            this.baseDensity = this.getDensity();
            if (absorptionSound == null || ejectionSound == null || failedEjectionSound == null)
            {
                this.loadSounds();
            }
        }

        private void loadSounds()
        {
            ejectionSound = AccretionGame.staticContent.Load<SoundEffect>("28839__junggle__btn029-2");
            //failedEjectionSound = AccretionGame.staticContent.Load<SoundEffect>("54405__korgms2000b__button-click");
            absorptionSound = AccretionGame.staticContent.Load<SoundEffect>("26893__vexst__reverse-snare-3-2");
        }

        public void usePowerUp(ref Field field)
        {
            if (this.powerUp != null)
            {
                this.powerUp.use(this, ref field);
            }
        }

        public void setPowerUp(PowerUp powerUp)
        {
            this.powerUp = powerUp;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 cameraFieldLocation, float zoomLevel, Vector2? lightSource, int windowWidth, int windowHeight)
        {
            //if there's music playing, pulse the player's size to the beat by fiddling with their density
            if (MediaPlayer.State == MediaState.Playing)
            {
                double bassPower = BeatDetector.getDecayingBassPower();
                double newDensity = (1.75 - 1.5 * bassPower) * this.baseDensity;
                this.setDensity(newDensity);
            }

            Vector2 playerScreenLocation = FieldAndScreenConversions.GetScreenLocation(this.getFieldLocation(), cameraFieldLocation, zoomLevel);
            Vector2 pointingDirection = Vector2.Zero;

#if WINDOWS
            MouseState currentMouseState = Mouse.GetState();
            Vector2 mouseLocation = new Vector2(currentMouseState.X, currentMouseState.Y);
            pointingDirection = playerScreenLocation - mouseLocation;
#elif XBOX
            if (GamepadHelper.activePlayerIndex.HasValue)
            {
                pointingDirection = GamepadHelper.getLeftThumbStickDirection(GamePad.GetState(GamepadHelper.activePlayerIndex.Value));
            }
#endif

#if !WINDOWS_PHONE
            //TODO: only recreate the helper line texture if the zoom level or player radius has changed
            //if (zoomLevel != oldZoomLevel || player.getRadius() != oldPlayerRadius)
            //{
            helperLineTexture = LineCreator.CreateLine((int)(this.getRadius() / zoomLevel), Math.Max((int)(this.getRadius() / Math.Sqrt(zoomLevel) / 2), (int)(this.getRadius() / zoomLevel * 2)), Color.Purple, spriteBatch.GraphicsDevice);
            //    oldZoomLevel = zoomLevel;
            //    oldPlayerRadius = player.getRadius();
            //}

            LineCreator.DrawLine(helperLineTexture, playerScreenLocation, pointingDirection, Color.Purple, spriteBatch);
#endif
            if (this.powerUp != null)
            {
                this.powerUp.drawAttached(spriteBatch, cameraFieldLocation, zoomLevel, lightSource, this, windowWidth, windowHeight);
            }

            base.draw(spriteBatch, cameraFieldLocation, zoomLevel, lightSource, windowWidth, windowHeight);
        }

        public void onUpdate(Field field) //should this be ref? does it matter for perf?
        {
            if (this.powerUp != null && this.getMass() > 0)
            {
                bool currentlyActive = this.powerUp.onUpdate(this, field);
                if (this.powerUp.getUses() < 1 && !currentlyActive)
                {
                    this.powerUp = null;
                }
            }
        }

        public override void absorbAndConserveMomentum(SpaceObject otherObject)
        {
            //same as the base, but works against PowerUps too
            lock (otherObject)
            {
                if (otherObject is PowerUp)
                {
                    this.setPowerUp(otherObject as PowerUp);
                    otherObject.pendingRemoval = true;
                }
                else
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
            }

            if (absorptionSound != null)
            {
                absorptionSound.Play();
            }
        }

        public SpaceObject ejectMass(int massToEject, Vector2 directionToEject)
        {
            if (this.getMass() > massToEject && massToEject > 0)
            {
                //calculate how far away to place the new object

                //TODO: figure out the proper speed to eject
                directionToEject.Normalize();
                Vector2 ejectionVector = directionToEject * this.ejectionSpeed + this.getVelocity();

                //Put it in the same location as the ejecting object for now; we will move it so it doesn't overlap the
                //original object once the radius is calculated (part of the constructor)
                RoundObject ejectedObject = new RoundObject(this.getFieldLocation(), ejectionVector, massToEject, this.getDensity());
                ejectedObject.setFieldLocation(this.getFieldLocation() + ((ejectedObject.getRadius() + this.getRadius() + 2) * directionToEject));

                //this factor lets us gain more momentum than we technically should. Less realism, more fun.
                float cheatFactor = 2f;

                this.addMomentum(-massToEject, ejectionVector * cheatFactor);
                this.addMass(-massToEject);

                if (ejectionSound != null)
                {
                    float pitch = 0.5f - this.getMass() * 1.5f / 6000f;
                    if (pitch < -1)
                    {
                        pitch = -1f;
                    }
                    if (pitch > 1)
                    {
                        pitch = 1f;
                    }

                    ejectionSound.Play(1f, pitch, 0f);
                }

                return ejectedObject;
            }
            else
            {
                if (this.getMass() > 0 && failedEjectionSound != null)
                {
                    failedEjectionSound.Play();
                }

                return null;
            }
        }
    }
}