using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Accretion.GraphicHelpers;

namespace Accretion.GameplayElements.Objects
{
    class RoundRadiatingObject : RoundObject
    {
        private static Texture2D sunTexture = AccretionGame.staticContent.Load<Texture2D>("circle200");

        public RoundRadiatingObject(Vector2 location, Vector2 velocity, int mass, int radius)
            : base(location, velocity, mass, radius)
        {
        }

        public RoundRadiatingObject(Vector2 location, Vector2 velocity, int mass, double density)
            : base(location, velocity, mass, density)
        {
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 cameraFieldLocation, float zoomLevel, Vector2? lightSource, int windowWidth, int windowHeight)
        {
            spriteBatch.Draw(sunTexture, FieldAndScreenConversions.GetScreenLocation(this.getFieldLocation(), cameraFieldLocation, zoomLevel), null, color, 0f, new Vector2(sunTexture.Width, sunTexture.Height) / 2, (float)Math.Max((float)this.getRadius() / zoomLevel / 100, .01), SpriteEffects.None, 0);
        }
    }
}
