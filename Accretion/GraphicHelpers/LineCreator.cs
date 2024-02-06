using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Accretion.GraphicHelpers
{
    class LineCreator
    {
        public static void DrawLine(int thickness, int length, Vector2 screenLocation, Vector2 angle, Color color, SpriteBatch spriteBatch)
        {
            color = Color.Lerp(color, Color.Transparent, 0.1f);
            Texture2D line = CreateLine(thickness, length, color, spriteBatch.GraphicsDevice);
            spriteBatch.Draw(line, screenLocation, null, color, (float)(Math.Atan2(angle.Y, angle.X) + Math.PI / 2), new Vector2(thickness / 2, 0), 1, SpriteEffects.None, 1);
        }

        public static void DrawLine(Texture2D lineTexture, Vector2 screenLocation, Vector2 angle, Color color, SpriteBatch spriteBatch)
        {
            color = Color.Lerp(color, Color.Transparent, 0.1f);
            spriteBatch.Draw(lineTexture, screenLocation, null, color, (float)(Math.Atan2(angle.Y, angle.X) + Math.PI / 2), new Vector2(lineTexture.Width / 2, 0), 1, SpriteEffects.None, 1);
        }

        public static Texture2D CreateLine(int thickness, int length, Color color, GraphicsDevice graphicsDevice)
        {
            if (thickness < 3)
            {
                thickness = 3;
            }
            else if (thickness > 2048)
            {
                thickness = 2048;
            }

            if (length < 20)
            {
                length = 20;
            }
            else if (length > 2048)
            {
                length = 2048;
            }

            Texture2D texture = new Texture2D(graphicsDevice, thickness, length);

            Color[] data = new Color[thickness * length];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = color;
            }

            texture.SetData(data);
            return texture;
        }
    }
}
