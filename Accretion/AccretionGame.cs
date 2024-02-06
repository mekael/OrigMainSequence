using Accretion.AudioHelpers;
using Accretion.GameplayElements.Objects;
using Accretion.GameplayObjects;
using Accretion.GraphicHelpers;
using Accretion.Input;
using Accretion.Levels;
using Accretion.Levels.NonlevelStates;
using Accretion.Levels.VictoryConditions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Accretion
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AccretionGame : Microsoft.Xna.Framework.Game, IDisposable
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameStatus gameStatus;
        private Song music;

        private Field gameField;
        private Level level;
        private VictoryCondition victoryCondition;

        private KeyboardState previousKeyboardState = Keyboard.GetState();
        private PlayerIndex? playerIndex = null;
        private GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
        private GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);
        private MouseState previousMouseState = Mouse.GetState();
        private MouseHelper mouseHandler = new MouseHelper();

        private const int MAX_ZOOM = 10000;
        private const int MIN_ZOOM = 1;

        //yuck, this is hacky
        public static ContentManager staticContent;

        public static SpriteFont font;

        public AccretionGame()
        {
 
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            staticContent = this.Content;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Add your initialization logic here
            MediaPlayer.IsVisualizationEnabled = true;
            this.IsMouseVisible = true;
            this.gameField = new Field();
            this.graphics.IsFullScreen = true;
            this.graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            //TODO: get rid of these debug settings
            //this.graphics.IsFullScreen = false;
            //this.graphics.PreferredBackBufferWidth = 1300;
            //this.graphics.PreferredBackBufferHeight = 900;
            this.graphics.ApplyChanges();
            base.Initialize();
            this.gameStatus = GameStatus.LevelSelectMenu;
#if WINDOWS_PHONE
            TouchPanel.EnabledGestures = GestureType.Pinch | GestureType.Tap | GestureType.Hold;
#endif
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (GraphicsDevice.DisplayMode.Width < 1280 || GraphicsDevice.DisplayMode.Height < 720)
            {
                font = staticContent.Load<SpriteFont>("MainMenuFontSmall");
            }
            else
            {
                font = staticContent.Load<SpriteFont>("MainMenuFont");
            }

            //use this.Content to load your game content here
            if (this.gameField == null)
            {
                this.gameField = new Field();
            }

            if (this.level == null)
            {
                this.level = new MenuBackground();
            }

            loadLevel(this.level);

            if (this.music != null)
            {
                SimplifiedMusicPlayer.PlaySong(this.music);
            }
            else
            {
                SimplifiedMusicPlayer.ContinuePlayingNonVictoryMusic();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.gameField.Dispose();
            this.gameField = null;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (this.playerIndex.HasValue)
            {
                gamePadState = GamePad.GetState(playerIndex.Value);
            }

            switch (this.gameStatus)
            {
                case GameStatus.InProgress:
                    this.updateSimulation();
                    gameStatus = victoryCondition.gameStatus(gameField);

                    if (gameStatus == GameStatus.Victory)
                    {
                        this.gameField.addGravitationalObjects(new List<SpaceObject>(1) { this.gameField.getPlayer() });
                    }

                    if (GamepadHelper.buttonIsNewlyPressed(Buttons.Back, gamePadState, previousGamePadState) ||
                        GamepadHelper.buttonIsNewlyPressed(Buttons.Start, gamePadState, previousGamePadState) ||
                        GamepadHelper.buttonIsNewlyPressed(Buttons.B, gamePadState, previousGamePadState) ||
                        KeyboardHelper.isNewlyPressed(Keys.Escape, keyboardState, previousKeyboardState))
                    {
                        this.gameStatus = GameStatus.LevelSelectMenu;
                    }

                    break;

                case GameStatus.OpeningText:
                    if (String.IsNullOrEmpty(this.level.openingText()) ||
#if WINDOWS
                        KeyboardHelper.anyKeyPressed(keyboardState, previousKeyboardState) ||
                        mouseHandler.anyMouseClick(mouseState, previousMouseState)
#elif XBOX
                        GamepadHelper.anyButtonPress(gamePadState, previousGamePadState)
#elif WINDOWS_PHONE
                        TouchscreenHelper.getTaps() != null
#endif
                        )
                    {
                        this.gameStatus = GameStatus.InProgress;
                    }

                    if (GamepadHelper.buttonIsNewlyPressed(Buttons.Back, gamePadState, previousGamePadState))
                    {
                        this.gameStatus = GameStatus.LevelSelectMenu;
                    }

                    break;

                case GameStatus.Victory:
                case GameStatus.Defeat:
                    if (KeyboardHelper.isNewlyPressed(Keys.Space, keyboardState, previousKeyboardState) ||
                        GamepadHelper.buttonIsNewlyPressed(Buttons.X, gamePadState, previousGamePadState) ||
                        TouchscreenHelper.getHolds() != null)
                    {
                        this.UnloadContent();
                        this.LoadContent();
                    }
                    else if (KeyboardHelper.isNewlyPressed(Keys.Escape, keyboardState, previousKeyboardState) ||
                        GamepadHelper.buttonIsNewlyPressed(Buttons.B, gamePadState, previousGamePadState) ||
                        GamepadHelper.buttonIsNewlyPressed(Buttons.Back, gamePadState, previousGamePadState))
                    {
                        this.gameStatus = GameStatus.LevelSelectMenu;
                    }
                    else
                    {
                        this.updateSimulation();
                    }

                    break;

                case GameStatus.LevelSelectMenu:
                    if (!playerIndex.HasValue)
                    {
                        selectPlayerIndex();
                    }

                    WP7Rotation.rotateVertical(graphics);

                    // Allows the game to exit
                    for (int i = 0; i < 4; i++)
                    {
                        if (GamePad.GetState((PlayerIndex)i).Buttons.Back == ButtonState.Pressed)
                        {
                            this.Exit();
                        }
                    }

                    if (this.level is MenuBackground)
                    {
                        this.updateSimulation();
                    }

                    if ((KeyboardHelper.isNewlyPressed(Keys.Escape, keyboardState, previousKeyboardState) ||
                        GamepadHelper.buttonIsNewlyPressed(Buttons.B, gamePadState, previousGamePadState) ||
                        GamepadHelper.buttonIsNewlyPressed(Buttons.Start, gamePadState, previousGamePadState))
                        && (this.level != null && !(this.level is MenuBackground)))
                    {
                        this.gameStatus = GameStatus.InProgress;
                        WP7Rotation.rotateHorizontal(graphics);
                    }
                    else
                    {
                        Level newLevel = null;
#if WINDOWS
                            newLevel = LevelSelectMenuHelper.keyboardAndMouseLevelSelect(keyboardState, previousKeyboardState, mouseState, previousMouseState);
#elif WINDOWS_PHONE
                            newLevel = LevelSelectMenuHelper.touchScreenInput();
#elif XBOX
                        if (playerIndex.HasValue)
                        {
                            newLevel = LevelSelectMenuHelper.gamePadLevelSelect(gamePadState, previousGamePadState);
                        }
#endif

                        if (newLevel != null)
                        {
                            if (newLevel is Quit)
                            {
                                this.Exit();
                            }
                            else if (newLevel is Cancel)
                            {
                                if (this.level != null && !(this.level is MenuBackground))
                                {
                                    this.gameStatus = GameStatus.InProgress;
                                    WP7Rotation.rotateHorizontal(graphics);
                                }
                            }
                            else if (newLevel is Credits)
                            {
                                this.gameStatus = GameStatus.Credits;
                            }
                            else
                            {
                                this.level = newLevel;
                                this.gameStatus = GameStatus.Starting;
                                WP7Rotation.rotateHorizontal(graphics);
                            }
                        }
                    }

                    break;

                case GameStatus.Starting:
                    this.UnloadContent();
                    this.LoadContent();
                    this.gameStatus = GameStatus.OpeningText;
                    break;

                case GameStatus.Credits:
                    if (this.level is MenuBackground)
                    {
                        this.updateSimulation();
                    }

                    if (KeyboardHelper.anyKeyPressed(keyboardState, previousKeyboardState) ||
                        mouseHandler.anyMouseClick(mouseState, previousMouseState) ||
                        GamepadHelper.anyButtonPress(gamePadState, previousGamePadState))
                    {
                        this.gameStatus = GameStatus.LevelSelectMenu;
                    }
                    CreditsHelper.increment++;
                    break;

                default:
                    throw new InvalidOperationException(String.Format("Not sure what to do with gamestate {0} in the Update function.", this.gameStatus));
            }

            this.previousKeyboardState = keyboardState;
            this.previousMouseState = mouseState;
            this.previousGamePadState = gamePadState;
            base.Update(gameTime);
        }

        private void selectPlayerIndex()
        {
            if (!playerIndex.HasValue)
            {
#if XBOX
                for (int i = 0; i < 4; i++)
                {
                    if (GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed)
                    {
                        this.playerIndex = (PlayerIndex)i;
                    }
                }
#else
                this.playerIndex = PlayerIndex.One;
#endif
                GamepadHelper.activePlayerIndex = this.playerIndex;

                if (playerIndex.HasValue)
                {
                    this.gamePadState = GamePad.GetState(playerIndex.Value);
                }
            }
        }



        private void updateSimulation()
        {
            if (this.gameField != null)
            {
                if (!(this.level is MenuBackground))
                {
                    checkForEjections(); //TODO: decide if this can be done less frequently, or maybe in Draw
                    checkForPowerUpUsage();
#if WINDOWS
                    this.previousMouseState = Mouse.GetState();
#endif
                }

                gameField.update();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if ((this.gameStatus == GameStatus.InProgress ||
                this.gameStatus == GameStatus.Defeat ||
                this.gameStatus == GameStatus.Victory)
                && this.gameField != null)
            {
                checkForZoom();
            }

            if (this.level != null && this.level.backgroundColor != null)
            {
                GraphicsDevice.Clear(this.level.backgroundColor);
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
            }

            spriteBatch.Begin();
            switch (gameStatus)
            {
                case GameStatus.InProgress:
                    if (gameField != null)
                    {
                        gameField.draw(this.spriteBatch, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
                    }
                    break;

                case GameStatus.OpeningText:
                    if (!String.IsNullOrEmpty(this.level.openingText()))
                    {
                        MessageWriter.displayMessageCentered(this.level.openingText(), font, spriteBatch, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
                    }

                    break;

                case GameStatus.Defeat:
                case GameStatus.Victory:
                    if (gameField != null)
                    {
                        gameField.draw(this.spriteBatch, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
                    }

                    string text = gameStatus == GameStatus.Victory ? this.level.successText() : this.level.defeatText();

                    if (string.IsNullOrEmpty(text))
                    {
                        text = gameStatus == GameStatus.Victory ? "Victory!" : "Failed!";
                    }

                    text = text + Environment.NewLine + Environment.NewLine + PlatformSpecificStrings.REPLAY_OR_BACK;

                    MessageWriter.displayMessageCentered(text, font, spriteBatch, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);

                    break;

                case GameStatus.LevelSelectMenu:
                    if (gameField != null)
                    {
                        gameField.draw(this.spriteBatch, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
                    }
                    LevelSelectMenuHelper.drawMenu(spriteBatch);
                    break;

                case GameStatus.Starting:
                    break;

                case GameStatus.Credits:
                    if (gameField != null)
                    {
                        gameField.draw(this.spriteBatch, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
                    }

                    CreditsHelper.draw(font, spriteBatch, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
                    break;

                default:
                    throw new InvalidOperationException(String.Format("Not sure what to do with gamestate {0} in the Draw function.", this.gameStatus));
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void loadLevel(Level level)
        {
            //try
            //{
            if (!String.IsNullOrEmpty(level.music))
            {
                this.music = Content.Load<Song>(level.music);
            }
            else
            {
                this.music = null;
            }

            if (level.initialZoom > 0)
            {
                this.gameField.initialZoomLevel = level.initialZoom;
                this.gameField.zoomLevel = level.initialZoom;
            }

            if (level.startingZoom > 0)
            {
                this.gameField.startingZoomLevel = level.startingZoom;
            }

            if (level.mapSize != null && level.mapSize != Vector2.Zero)
            {
                this.gameField.setMapSize(level.mapSize);
            }

            this.victoryCondition = level.victoryCondition;
            this.gameField.gravity = level.gravitationalLaw;
            this.gameField.wrapEdges = level.wrapEdges;

            List<SpaceObject> collisionObjects = level.spaceObjects();
            if (collisionObjects != null)
            {
                this.gameField.addSpaceObjects(collisionObjects);
            }

            List<SpaceObject> gravitationalObjects = level.gravitationalObjects();
            if (gravitationalObjects != null)
            {
                this.gameField.addGravitationalObjects(gravitationalObjects);
            }

            this.gameField.setPlayer(level.player());

            if (level.cameraLockedOnPlayer)
            {
                this.gameField.cameraLockedObject = gameField.getPlayer();
            }

            this.gameStatus = GameStatus.InProgress;

            if (level.collisionDetection != null)
            {
                this.gameField.collisionDetection = level.collisionDetection;
            }
            //}
            //catch (Exception e)
            //{
            //    //TODO: display some error
            //}
        }

        private void checkForEjections()
        {
            if (this.IsActive)
            {
                if (gameField != null && gameField.getPlayer() != null)
                {
                    Vector2? ejectionDirection = null;
#if WINDOWS
                    MouseState currentMouseState = Mouse.GetState();
                    Vector2? ejectionClickLocation = mouseHandler.getEjectionMouseClick(currentMouseState, this.previousMouseState);
                    if (ejectionClickLocation.HasValue)
                    {
                        ejectionDirection = gameField.getScreenLocation(gameField.getPlayer()) - ejectionClickLocation;
                    }
#elif XBOX
                    ejectionDirection = GamepadHelper.getEjectionButtonPress(this.gamePadState, this.previousGamePadState);
#elif WINDOWS_PHONE
                    List<Vector2> taps = TouchscreenHelper.getTaps();
                    if (taps != null && taps.Count > 0)
                    {
                        ejectionDirection = gameField.getScreenLocation(gameField.getPlayer()) - taps[0];
                    }
#endif
                    if (ejectionDirection.HasValue && ejectionDirection.Value != Vector2.Zero)
                    {
                        //TODO: find the right fraction to eject
                        int massToEject = gameField.getPlayer().getMass() / 32;
                        if (massToEject <= 0 && gameField.getPlayer().getMass() > 1)
                        {
                            massToEject = 1;
                        }

                        SpaceObject ejectedMass = gameField.getPlayer().ejectMass(massToEject, ejectionDirection.Value);

                        if (ejectedMass != null)
                        {
                            gameField.addSpaceObject(ejectedMass);
                        }
                    }
                }
            }
        }

        private void checkForPowerUpUsage()
        {
            if (gameField != null && gameField.getPlayer() != null)
            {
                Vector2? mousePowerDirections = mouseHandler.getRightMouseClick(Mouse.GetState(), this.previousMouseState);
                bool gamepadPowerUsed = GamepadHelper.buttonIsNewlyPressed(Buttons.RightTrigger, this.gamePadState, this.previousGamePadState);

                if (mousePowerDirections != null || gamepadPowerUsed)
                {
                    this.gameField.getPlayer().usePowerUp(ref gameField);
                }
            }
        }

        private void checkForZoom()
        {
#if WINDOWS
            int scrollWheelChange = this.mouseHandler.getScrollWheelChange();
            if (scrollWheelChange != 0)
            {
                double zoomChange = (double)scrollWheelChange / 300 * this.gameField.zoomLevel / 3d;
                this.gameField.zoomLevel += (float)zoomChange;
            }
#elif XBOX
            float change = previousGamePadState.ThumbSticks.Right.Y * this.gameField.zoomLevel / 20f;
            this.gameField.zoomLevel -= change;
#elif WINDOWS_PHONE
            float? change = TouchscreenHelper.getPinchRatio();
            if (change.HasValue)
            {
                float newZoom = this.gameField.zoomLevel / change.Value;
                this.gameField.zoomLevel = newZoom;
            }
#endif

            if (this.gameField.zoomLevel < MIN_ZOOM)
            {
                this.gameField.zoomLevel = MIN_ZOOM;
            }
            else if (this.gameField.zoomLevel > MAX_ZOOM)
            {
                this.gameField.zoomLevel = MAX_ZOOM;
            }
        }

        void IDisposable.Dispose()
        {
            this.gameField.Dispose();
            base.Dispose();
        }
    }
}