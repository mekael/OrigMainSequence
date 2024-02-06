using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Accretion.Input
{
    class KeyboardHelper
    {
        public static bool anyKeyPressed(KeyboardState keyboardState, KeyboardState previousKeyboardState)
        {
            foreach (Keys key in keyboardState.GetPressedKeys())
            {
                if (previousKeyboardState.IsKeyUp(key))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool isNewlyPressed(Keys key, KeyboardState keyboardState, KeyboardState previousKeyBoardState)
        {
            return keyboardState.IsKeyDown(key) && previousKeyBoardState.IsKeyUp(key);
        }
    }
}
