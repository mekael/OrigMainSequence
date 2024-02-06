using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayObjects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Accretion.GraphicHelpers;

namespace Accretion.GameplayElements.Objects.PowerUps
{
    abstract class PowerUp : SpaceObject
    {
        protected int uses;
        protected DateTime lastUsed;
        protected SpriteFont font = null;
        protected Vector2 fontCenter;
        protected float fontZoomFactor = 1;
        protected static Texture2D bubble = AccretionGame.staticContent.Load<Texture2D>("bubble200");
        protected static Vector2 textureCenter = new Vector2(bubble.Width, bubble.Height) / 2;

        public abstract Char getFieldDisplayCharacter();

        //TODO: get rid of these silly const
        public PowerUp(int uses) : base(Vector2.Zero, Vector2.Zero, 1, 300)
        {
            this.uses = uses;
            this.font = AccretionGame.font;
            Char displayCharacter = this.getFieldDisplayCharacter();
            this.fontCenter = font.MeasureString(displayCharacter.ToString()) / 2;
            this.fontZoomFactor = font.MeasureString(displayCharacter.ToString()).Length() / 4;
        }

        public PowerUp(int uses, Vector2 location, Vector2 velocty)
            : base(location, velocty, 1, 300)
        {
            this.uses = uses;
            this.font = AccretionGame.font;
            Char displayCharacter = this.getFieldDisplayCharacter();
            this.fontCenter = font.MeasureString(displayCharacter.ToString()) / 2;
            this.fontZoomFactor = font.MeasureString(displayCharacter.ToString()).Length() / 4;
        }

        public virtual void use(PlayerObject player, ref Field field)
        {
            if (player != null && player.getMass() > 0 && !player.pendingRemoval)
            {
                lastUsed = DateTime.UtcNow;
                SoundEffect soundEffect = getSoundEffect();
                if (soundEffect != null)
                {
                    soundEffect.Play(1f, 0f, 0f);
                }

                this.uses--;
            }
        }

        public int getUses()
        {
            return this.uses;
        }

        public void DisplayCount(SpriteBatch spriteBatch, int windowWidth, int windowHeight)
        {
            MessageWriter.displayMessageTopRight(String.Format("{0} to use {1}: {2} left", PlatformSpecificStrings.POWERUP_CONTROL, this.getPowerName(), this.uses), font, spriteBatch, windowWidth, windowHeight);
        }

        public abstract String getPowerName();

        public abstract SoundEffect getSoundEffect();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="field"></param>
        /// <returns>returns true if the powerup is currently in use, or false if it is not</returns>
        public abstract bool onUpdate(PlayerObject player, Field field);

        public virtual void drawAttached(SpriteBatch spriteBatch, Vector2 cameraFieldLocation, float zoomLevel, Vector2? lightSource, PlayerObject player, int windowWidth, int windowHeight)
        {
            this.DisplayCount(spriteBatch, windowWidth, windowHeight);
        }

        public override List<SpaceObject> explode()
        {
            return new List<SpaceObject>();
        }

        public override void absorbAndConserveMomentum(SpaceObject otherObject)
        {
            if (otherObject is PlayerObject)
            {
                PlayerObject player = (PlayerObject)otherObject;
                player.setPowerUp(this);
                this.pendingRemoval = true;
            }
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 cameraLocation, float zoomLevel, Vector2? lightSource, int windowWidth, int windowHeight)
        {

            float rotation = (float)Math.PI;
            if (lightSource.HasValue)
            {
                Vector2 direction = lightSource.Value - this.getFieldLocation();
                rotation = (float)Math.Atan2(direction.X, -direction.Y);
            }

            char displayChar = this.getFieldDisplayCharacter();

            string displayString = displayChar.ToString();
            spriteBatch.DrawString(font, displayString, FieldAndScreenConversions.GetScreenLocation(this.getFieldLocation(), cameraLocation, zoomLevel), Color.DarkOrange, rotation + (float)Math.PI, this.fontCenter, fontZoomFactor / zoomLevel, SpriteEffects.None, 0);

            spriteBatch.Draw(bubble, FieldAndScreenConversions.GetScreenLocation(this.getFieldLocation(), cameraLocation, zoomLevel), null, color, rotation, textureCenter, (float)Math.Max((float)this.getRadius() / zoomLevel / 100, .01), SpriteEffects.None, 0);
        }
    }
}
