using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace FallingWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D _tileTexture, _jumperTexture;
        private Jumper jumper;

        private Board board;

        private SpriteFont debugFont;

        enum GameState
        {
            MainMenu,
            SelectPlayer,
            Playing
        }

        GameState CurrentGameState = GameState.MainMenu;
        int screenWidth = 960;
        int screenHeight = 640;

        Button btnPlay;
        Button btnExit;
        Button btnPlayerD;
        Button btnPlayerS;
        Button btnBack;
        Button btnResumePaused;
        Button btnExitPaused;
        Boolean paused = false;
        Rectangle pausedRectangle;
        Texture2D pauseTexture;

        KeyboardState oldState, newState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            IsMouseVisible = true;

            btnPlay = new Button(Content.Load<Texture2D>("PlayButtonTransparent"), graphics.GraphicsDevice, 200, 50);
            btnPlay.setPosition(new Vector2(100, 500));
            btnExit = new Button(Content.Load<Texture2D>("QuitButton"), graphics.GraphicsDevice, 200, 50);
            btnExit.setPosition(new Vector2(660, 500));

            btnResumePaused = new Button(Content.Load<Texture2D>("ResumeButton"), graphics.GraphicsDevice, 200, 50);
            btnResumePaused.setPosition(new Vector2(375, 240));
            btnExitPaused = new Button(Content.Load<Texture2D>("MenuButton"), graphics.GraphicsDevice, 200, 50);
            btnExitPaused.setPosition(new Vector2(375, 300));

            btnPlayerD = new Button(Content.Load<Texture2D>("Deadpool 256"), graphics.GraphicsDevice, 256, 256);
            btnPlayerD.setPosition(new Vector2(100, 300));

            btnPlayerS = new Button(Content.Load<Texture2D>("Spiderman 256"), graphics.GraphicsDevice, 256, 256);
            btnPlayerS.setPosition(new Vector2(500, 300));

            pausedRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            pauseTexture = Content.Load<Texture2D>("Pause");

            _tileTexture = Content.Load<Texture2D>("tile");
            jumper = new Jumper(null, null, new Vector2(460, 525), spriteBatch);
            board = new Board(spriteBatch, _tileTexture, 15, 10);

            debugFont = Content.Load<SpriteFont>("DebugFont");

            oldState = Keyboard.GetState();
        }

        protected override void UnloadContent()
        {
           
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            MouseState mouse = Mouse.GetState();

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    if (btnPlay.isCLicked)
                    {
                        Thread.Sleep(1500);
                        btnPlay.isCLicked = false;
                        CurrentGameState = GameState.SelectPlayer;
                    }

                    if (btnExit.isCLicked)
                        this.Exit();

                    btnPlay.Update(mouse);
                    btnExit.Update(mouse);
                    break;
                case GameState.SelectPlayer:
                    if (btnPlayerD.isCLicked)
                    {
                        btnPlayerD.isCLicked = false;
                        jumper.Texture = Content.Load<Texture2D>("DeadpoolLittle");
                        jumper.TextureGauche = Content.Load<Texture2D>("DeadpoolLittle");
                        jumper.TextureDroite = Content.Load<Texture2D>("DeadpoolLittleDroite");
                        CurrentGameState = GameState.Playing;
                    }

                    if (btnPlayerS.isCLicked)
                    {
                        btnPlayerS.isCLicked = false;
                        jumper.Texture = Content.Load<Texture2D>("SpidermanLittle");
                        jumper.TextureGauche = Content.Load<Texture2D>("SpidermanLittle");
                        jumper.TextureDroite = Content.Load<Texture2D>("SpidermanLittleDroite");
                        CurrentGameState = GameState.Playing;
                    }
                    btnPlayerD.Update(mouse);
                    btnPlayerS.Update(mouse);
                    break;
                case GameState.Playing:
                    newState = Keyboard.GetState();
                    if (!paused)
                    {
                        if (newState.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape))
                            paused = true;
                        jumper.Update(gameTime);
                    }
                    else
                    {
                        if (btnResumePaused.isCLicked || (newState.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape)))
                        {
                            btnResumePaused.isCLicked = false;
                            paused = false;
                        }

                        if (btnExitPaused.isCLicked)
                        {
                            btnExitPaused.isCLicked = false;
                            btnPlay.isCLicked = false;
                            paused = false;
                            CurrentGameState = GameState.MainMenu;
                        }
                        btnResumePaused.Update(mouse);
                        btnExitPaused.Update(mouse);
                    }
                    oldState = newState;
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(Content.Load<Texture2D>("MainMenu"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    btnPlay.Draw(spriteBatch);
                    btnExit.Draw(spriteBatch);
                    break;
                case GameState.SelectPlayer:
                    spriteBatch.Draw(Content.Load<Texture2D>("SelectPlayer"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    btnPlayerD.Draw(spriteBatch);
                    btnPlayerS.Draw(spriteBatch);
                    break;
                case GameState.Playing:
                    string positionInText = string.Format("Position of Jumper: ({0:0.0}, {1:0.0})", jumper.Position.X, jumper.Position.Y);
                    string movementInText = string.Format("Current movement: ({0:0.0}, {1:0.0})", jumper.Movement.X, jumper.Movement.Y);
                    GraphicsDevice.Clear(Color.WhiteSmoke);
                    base.Draw(gameTime);
                    board.Draw();
                    if (paused)
                    {
                        spriteBatch.Draw(pauseTexture, pausedRectangle, Color.White);
                        btnExitPaused.Draw(spriteBatch);
                        btnResumePaused.Draw(spriteBatch);
                    }
                    spriteBatch.DrawString(debugFont, positionInText, new Vector2(10, 0), Color.Black);
                    spriteBatch.DrawString(debugFont, movementInText, new Vector2(10, 20), Color.Black);
                    jumper.Draw();
                    
                    break;
            }

            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
