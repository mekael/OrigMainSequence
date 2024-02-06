using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Accretion.GraphicHelpers
{
    class CircleCreator
    {
        public static void DrawCircle(int radius, Vector2 location, Color color, SpriteBatch spriteBatch)
        {
            Texture2D circle = CreateCircle(radius, spriteBatch.GraphicsDevice);
            spriteBatch.Draw(circle, location - new Vector2(circle.Width, circle.Height) / 2, color);
        }

        public static Texture2D CreateCircle(int radius, GraphicsDevice graphicsDevice)
        {
            int outerRadius = TextureSize(radius);
            Texture2D texture = new Texture2D(graphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        public static int TextureSize(int radius)
        {
            return radius * 2 + 2; // So circle doesn't go out of bounds
        }
    }
}
