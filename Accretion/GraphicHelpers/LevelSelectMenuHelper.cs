using Accretion.AudioHelpers;
using Accretion.Input;
using Accretion.Levels;
using Accretion.Levels.NonlevelStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Accretion.GraphicHelpers
{
    internal class LevelSelectMenuHelper
    {
        private static int levelNumber = 0;
        private static SpriteFont font;
        private static readonly Texture2D cursor;
        private static Vector2? TopLeftOffset;
        private static Vector2 MenuItemSpacing;
        private static int scrollOffset = 0;

        private const String SCROLL_CURSOR = "^";
        private static Vector2 scrollCursorSize;

        //We want to make sure that the "Main Sequence" title text doesn't overlap any level names.
        //So keep track of the furthest right point on the screen they reach so we can move
        //the title if necessary.
        private static float furtherRightMenuText;

        static LevelSelectMenuHelper()
        {
            cursor = AccretionGame.staticContent.Load<Texture2D>("circle200");
        }

        public static Level getLevel(int levelNumber)
        {
 
            switch (levelNumber)
            {
                case 0:
                    return new Introduction();

                case 1:
                    return new Heliocentric();

                case 2:
                    return new Rings();

                case 3:
                    return new BinaryStar();

                case 4:
                    return new TheFunnel();

                case 5:
                    return new Nemesis();
                case 6:
                    return new CounterRevolutionary();

                case 7:
                    return new SpiralArms();
                case 8:
                    return new TwinPeaks();
                case 9:
                    return new WhirlPool();
                case 10:
                    return new Nebulous();
                case 11:
                    return new Shell();
                case 12:
                    return new MeteorDefense();
                case 13:
                    return new Cancel();

                case 14:
                    CreditsHelper.increment = 0;
                    return new Credits();

                case 15:
                    return new Quit();

                case 16:
                    return new AntigravDebugLevel();
                case 17:
                    return new Debug();
                case 18:
                    return new DebugShrinkField();
                case 19:
                    return new DebugSpeedBoost();
                case 20:
                    return new GravDebugLevel();
                default:
                    throw new IndexOutOfRangeException(String.Format("Unable to load level {0}", levelNumber));
            }
        }

        public static readonly List<String> levelNames = new List<String>
        {
            "Introduction",
            "Heliocentric",
            "Michael's Halo",
            "Binary Math",
            "The Funnel",
            "Nemesis",
            "Counter Revolutionary",
            "Spiral Arms",
            "Twin Peaks",
            "Whirlpool",
            "Nebulous",
            "Dyson's Shell Game",
            "Deconstructionist",
            "~Cancel~",
            "~Credits~",
            "~Exit~",

            "AntigravDebugLevel",
            "Debug",
            "DebugShrinkField",
            "DebugSpeedBoost",
            "GravDebugLevel",
        };

        public static Level gamePadLevelSelect(GamePadState gamePadState, GamePadState previousGamePadState)
        {
            if (GamepadHelper.buttonIsNewlyPressed(Buttons.A, gamePadState, previousGamePadState))
            {
                return getLevel(levelNumber);
            }
            else if (GamepadHelper.shouldScrollDown(gamePadState, previousGamePadState))
            {
                incrementLevel();
            }
            else if (GamepadHelper.shouldScrollUp(gamePadState, previousGamePadState))
            {
                decrementLevel();
            }

            return null;
        }

#if WINDOWS_PHONE
        public static Level touchScreenInput()
        {
            List<Vector2> touches = TouchscreenHelper.getTaps();
            if (touches != null && touches.Count > 0)
            {
                for (int i = 0; i <levelNames.Count; i++)
                {
                    BoundingBox? messageBounds = getMessageBoundingBox(i);
                    foreach (Vector2 touch in touches)
                    {
                        if (messageBounds.HasValue && messageBounds.Value.Contains(new Vector3(touch, 0)) == ContainmentType.Contains)
                        {
                            levelNumber = i;
                            return getLevel(levelNumber);
                        }
                    }
                }
            }

            return null;
        }
#endif

        public static Level keyboardAndMouseLevelSelect(KeyboardState keyboardState, KeyboardState previousKeyboardState, MouseState mouseState, MouseState previousMouseState)
        {
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                return getLevel(levelNumber);
            }
            else if (keyboardState.IsKeyDown(Keys.Down) && !previousKeyboardState.IsKeyDown(Keys.Down))
            {
                incrementLevel();
            }
            else if (keyboardState.IsKeyDown(Keys.Up) && !previousKeyboardState.IsKeyDown(Keys.Up))
            {
                decrementLevel();
            }
            else //mouse handling
            {
                bool mouseIsOverMessage = false;
                //Parallel.For(0, levelNames.Count, i =>
                for (int i = 0; i < levelNames.Count; i++)
                {
                    BoundingBox? messageBounds = getMessageBoundingBox((int)i);
                    if (messageBounds.HasValue && messageBounds.Value.Contains(new Vector3(mouseState.X, mouseState.Y, 0)) == ContainmentType.Contains)
                    {
                        levelNumber = i;
                        mouseIsOverMessage = true;
                    }
                    //});
                }

                if (mouseIsOverMessage && mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed)
                {
                    return getLevel(levelNumber);
                }
            }

            return null;
        }

        private static void incrementLevel()
        {
            levelNumber = ++levelNumber % levelNames.Count;
        }

        private static void decrementLevel()
        {
            levelNumber = --levelNumber % levelNames.Count;
            if (levelNumber < 0)
            {
                levelNumber = levelNames.Count - 1;
            }
        }

        public static void drawMenu(SpriteBatch spriteBatch)
        {
            if (TopLeftOffset == null)
            {
                TopLeftOffset = calculateTopLeftOffset(spriteBatch.GraphicsDevice.DisplayMode);
            }

            setFont(spriteBatch);

            Vector2 selectedLevelScreenLocation = getMessageLocation(levelNumber, scrollOffset);
            while (selectedLevelScreenLocation.Y > spriteBatch.GraphicsDevice.DisplayMode.Height * .85)
            {
                scrollOffset--;
                selectedLevelScreenLocation = getMessageLocation(levelNumber, scrollOffset);
            }
            while (selectedLevelScreenLocation.Y < spriteBatch.GraphicsDevice.DisplayMode.Height * .15)
            {
                scrollOffset++;
                selectedLevelScreenLocation = getMessageLocation(levelNumber, scrollOffset);
            }

#if XBOX
            if (!GamepadHelper.activePlayerIndex.HasValue)
            {
                String pressStartMessage = "Press [Start]";
                Vector2 startSize = font.MeasureString(pressStartMessage);
                Vector2 startLocation = new Vector2(spriteBatch.GraphicsDevice.DisplayMode.Width /2, spriteBatch.GraphicsDevice.DisplayMode.Height * 3 / 4);
                Vector2 startOrigin = startSize / 2;
                spriteBatch.DrawString(font, pressStartMessage, startLocation, Color.White, 0, startOrigin, 2, SpriteEffects.None, 0);
            }
            else
            {
#endif
            spriteBatch.DrawString(font, "Levels:", getMessageLocation(0, scrollOffset), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            for (int i = 0; i < levelNames.Count; i++)
            {
                Vector2 messageSize = font.MeasureString(levelNames[i]);
                Vector2 messageLocation = getMessageLocation(i + 1, scrollOffset);  //+1 due to the "levels" header
                float rightEdge = messageLocation.X + messageSize.X;
                if (rightEdge > furtherRightMenuText)
                {
                    furtherRightMenuText = rightEdge;
                }

                spriteBatch.DrawString(font, levelNames[i], messageLocation, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                //draw the cursor
                if (i == levelNumber)
                {
                    float cursorZoom = .25f * (float)BeatDetector.getDecayingBassPower();
                    cursorZoom = Math.Max(cursorZoom, 0.08f); //make sure the cursor doesn't get TOO small
                    spriteBatch.Draw(cursor, MenuItemSpacing * (i + 1 + scrollOffset) + new Vector2(messageSize.X + 10, messageSize.Y / 2) + TopLeftOffset.Value, null, Color.Red, 0f, new Vector2(0, cursor.Height) / 2, cursorZoom, SpriteEffects.None, 0);
                }
            }

            //draw the ▲ or ▼ hints if the menu extends past the screen
            if (getMessageLocation(0, scrollOffset).Y < spriteBatch.GraphicsDevice.DisplayMode.Height * .15)
            {
                spriteBatch.DrawString(font, "^", new Vector2(spriteBatch.GraphicsDevice.DisplayMode.Width * 0.12f, getMessageLocation(0, 0).Y), Color.Red, 0f, new Vector2(scrollCursorSize.X, 0), 2, SpriteEffects.None, 0);
            }
            if (getMessageLocation(levelNames.Count, scrollOffset).Y > spriteBatch.GraphicsDevice.DisplayMode.Height * .9)
            {
                //draw a "^" but flip it vertically
                spriteBatch.DrawString(font, "^", new Vector2(spriteBatch.GraphicsDevice.DisplayMode.Width * 0.12f, spriteBatch.GraphicsDevice.DisplayMode.Height * 0.87f - scrollCursorSize.Y / 2), Color.Red, 0f, new Vector2(scrollCursorSize.X, scrollCursorSize.Y / 2), 2, SpriteEffects.FlipVertically, 0);
            }
#if XBOX
            }
#endif


            String title = "Main Sequence";


            Vector2 titleSize = font.MeasureString(title);
            Vector2 titleOrigin = titleSize / 2;
            titleSize *= 2; //2x since we will be magnifying the title font in the draw function too
#if WINDOWS_PHONE
            Vector2 titleCenterLocation = Vector2.Zero;
            titleOrigin = Vector2.Zero;
#else
            Vector2 titleCenterLocation = new Vector2(spriteBatch.GraphicsDevice.DisplayMode.Width / 2, spriteBatch.GraphicsDevice.DisplayMode.Height / 2);

            while (titleCenterLocation.X - titleSize.X / 2 < furtherRightMenuText)
            {
                titleCenterLocation.X += 100;
            }
#endif
            spriteBatch.DrawString(font, title, titleCenterLocation, Color.White, 0, titleOrigin, 2, SpriteEffects.None, 0);
        }

        private static void setFont(SpriteBatch spriteBatch)
        {
            if (font == null)
            {
                if (spriteBatch.GraphicsDevice.DisplayMode.Width < 1280 || spriteBatch.GraphicsDevice.DisplayMode.Height < 720)
                {
                    font = AccretionGame.staticContent.Load<SpriteFont>("MainMenuFontSmall");
                }
                else
                {
                    font = AccretionGame.staticContent.Load<SpriteFont>("MainMenuFont");
                }

                MenuItemSpacing = font.MeasureString("A") * new Vector2(0, 1);
                scrollCursorSize = font.MeasureString(SCROLL_CURSOR);
            }
        }

        //Returns the top left of a message
        private static Vector2 getMessageLocation(int messageNumber, int scrollOffset)
        {
            Vector2 messageLocation = MenuItemSpacing * (messageNumber + scrollOffset);
            if (TopLeftOffset.HasValue)
            {
                messageLocation += TopLeftOffset.Value;
            }

            return messageLocation;
        }

        private static Vector2 calculateTopLeftOffset(DisplayMode displayMode)
        {
//#if XBOX
           // return new Vector2(displayMode.Width, displayMode.Height) * 13 / 100;
//#else
            return new Vector2(50, 75);
//#endif
        }

        private static BoundingBox? getMessageBoundingBox(int messageNumber)
        {
            if (font == null)
            {
                return null;
            }
            else
            {
                return new BoundingBox(new Vector3(getMessageLocation(messageNumber + 1, scrollOffset), 0), new Vector3(getMessageLocation(messageNumber + 1, scrollOffset) + font.MeasureString(levelNames[messageNumber]), 0));
            }
        }
    }
}