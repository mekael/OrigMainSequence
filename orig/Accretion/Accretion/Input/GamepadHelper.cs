using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Accretion.Input
{
    class GamepadHelper
    {
        private static readonly Vector2 HORIZONTAL_FLIP = new Vector2(-1, 1);
        private static Vector2 lastThumbPadDirection;
        public static readonly Buttons[] ALL_BUTTONS = new Buttons[] { Buttons.A, Buttons.B, Buttons.X, Buttons.Y, Buttons.Back, Buttons.LeftShoulder, Buttons.LeftStick, Buttons.LeftTrigger, Buttons.RightShoulder, Buttons.RightStick, Buttons.RightTrigger, Buttons.Start };
        public static PlayerIndex? activePlayerIndex = null;

        private static DateTime? downScrollStarted = null;
        private static DateTime? upScrollStarted = null;
        private static readonly TimeSpan waitBetweenScrolls = TimeSpan.FromSeconds(0.15);

        public static bool anyButtonPress(GamePadState gamePadState, GamePadState previousGamePadState)
        {
            if (gamePadState.Buttons != previousGamePadState.Buttons)
            {
                foreach (Buttons button in ALL_BUTTONS)
                {
                    if (buttonIsNewlyPressed(button, gamePadState, previousGamePadState))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static Vector2? getEjectionButtonPress(GamePadState gamePadState, GamePadState previousGamePadState)
        {
            Vector2? ejectionDirection = null;

            if (buttonIsNewlyPressed(Buttons.A, gamePadState, previousGamePadState))
            {
                ejectionDirection = getLeftThumbStickDirection(gamePadState);
            }

            return ejectionDirection;
        }

        public static bool buttonIsNewlyPressed(Buttons button, GamePadState gamePadState, GamePadState previousGamePadState)
        {
            return gamePadState.IsButtonDown(button) && previousGamePadState.IsButtonUp(button);
        }

        public static bool thumbStickMovedUp(GamePadState gamePadState, GamePadState previousGamePadState)
        {
            return (gamePadState.ThumbSticks.Left.Y > 0.1f && previousGamePadState.ThumbSticks.Left.Y < 0.1f) ||
            (gamePadState.ThumbSticks.Right.Y > 0.1f && previousGamePadState.ThumbSticks.Right.Y < 0.1f);
        }

        public static bool thumbStickMovedDown(GamePadState gamePadState, GamePadState previousGamePadState)
        {
            return (gamePadState.ThumbSticks.Left.Y < -0.1f && previousGamePadState.ThumbSticks.Left.Y > -0.1f) ||
                (gamePadState.ThumbSticks.Right.Y < -0.1f && previousGamePadState.ThumbSticks.Right.Y > -0.1f);
        }

        public static Vector2 getLeftThumbStickDirection(GamePadState gamePadState)
        {
            Vector2 direction = gamePadState.ThumbSticks.Left * HORIZONTAL_FLIP;

            if (direction.Length() < 0.2f && lastThumbPadDirection != null)
            {
                direction = lastThumbPadDirection;
            }

            lastThumbPadDirection = direction;

            return direction;
        }

        public static bool shouldScrollDown(GamePadState gamePadState, GamePadState previousGamePadState)
        {
            if (GamepadHelper.buttonIsNewlyPressed(Buttons.DPadDown, gamePadState, previousGamePadState) ||
                GamepadHelper.thumbStickMovedDown(gamePadState, previousGamePadState) ||
                ((gamePadState.ThumbSticks.Left.Y < -0.1 || gamePadState.ThumbSticks.Right.Y < -0.1 || gamePadState.IsButtonDown(Buttons.DPadDown)) && downScrollStarted.HasValue && DateTime.UtcNow - downScrollStarted > waitBetweenScrolls))
            {
                downScrollStarted = DateTime.UtcNow;
                return true;
            }
            else if ((gamePadState.ThumbSticks.Left.Y < -0.1 || gamePadState.ThumbSticks.Right.Y < -0.1 || gamePadState.IsButtonDown(Buttons.DPadDown))  && downScrollStarted.HasValue)
            {
                return false;
            }
            else
            {
                downScrollStarted = null;
                return false;
            }
        }

        public static bool shouldScrollUp(GamePadState gamePadState, GamePadState previousGamePadState)
        {
            if (GamepadHelper.buttonIsNewlyPressed(Buttons.DPadUp, gamePadState, previousGamePadState) ||
                GamepadHelper.thumbStickMovedUp(gamePadState, previousGamePadState) ||
                ((gamePadState.ThumbSticks.Left.Y > 0.1 || gamePadState.ThumbSticks.Right.Y > 0.1 || gamePadState.IsButtonDown(Buttons.DPadUp)) && upScrollStarted.HasValue && DateTime.UtcNow - upScrollStarted > waitBetweenScrolls))
            {
                upScrollStarted = DateTime.UtcNow;
                return true;
            }
            else if ((gamePadState.ThumbSticks.Left.Y > 0.1 || gamePadState.ThumbSticks.Right.Y > 0.1 || gamePadState.IsButtonDown(Buttons.DPadUp)) && upScrollStarted.HasValue)
            {
                return false;
            }
            else
            {
                upScrollStarted = null;
                return false;
            }
        }
    }
}
