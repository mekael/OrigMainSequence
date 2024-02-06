using Microsoft.Xna.Framework;

namespace Accretion.GraphicHelpers
{
    internal class WP7Rotation
    {
        public static void rotateVertical(GraphicsDeviceManager graphics)
        {
#if WINDOWS_PHONE
            if (graphics.PreferredBackBufferHeight < graphics.PreferredBackBufferWidth)
            {
                rotate(graphics);
            }
#endif
        }

        public static void rotateHorizontal(GraphicsDeviceManager graphics)
        {
#if WINDOWS_PHONE
            if (graphics.PreferredBackBufferWidth < graphics.PreferredBackBufferHeight)
            {
                rotate(graphics);
            }
#endif
        }

        private static void rotate(GraphicsDeviceManager graphics)
        {
            int temp = graphics.PreferredBackBufferWidth;
            graphics.PreferredBackBufferWidth = graphics.PreferredBackBufferHeight;
            graphics.PreferredBackBufferHeight = temp;
            graphics.ApplyChanges();
        }
    }
}