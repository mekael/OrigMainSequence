using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Accretion.Input
{
    internal class MouseHelper
    {
        private int oldMouseScrollState = Mouse.GetState().ScrollWheelValue;

        public Vector2? getEjectionMouseClick(MouseState current, MouseState previous)
        {
            Vector2? ejectionDirection = null;

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton != ButtonState.Pressed)
            {
                ejectionDirection = new Vector2(current.X, current.Y);
            }

            return ejectionDirection;
        }

        public Vector2? getRightMouseClick(MouseState current, MouseState previous)
        {
            Vector2? ejectionDirection = null;

            if (current.RightButton == ButtonState.Pressed && previous.RightButton != ButtonState.Pressed)
            {
                ejectionDirection = new Vector2(current.X, current.Y);
            }

            return ejectionDirection;
        }

        public int getScrollWheelChange()
        {
            MouseState currentMouseState = Mouse.GetState();
            int scrollWheelChange = oldMouseScrollState - currentMouseState.ScrollWheelValue;
            oldMouseScrollState = currentMouseState.ScrollWheelValue;

            return scrollWheelChange;
        }

        public bool anyMouseClick(MouseState mouseState, MouseState previousMouseState)
        {
            bool leftClicked = mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed;
            bool rightClicked = mouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton != ButtonState.Pressed;

            return leftClicked || rightClicked;
        }
    }
}