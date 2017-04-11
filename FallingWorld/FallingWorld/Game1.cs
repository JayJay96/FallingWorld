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

        cButton btnPlay;
        cButton btnExit;
        cButton btnPlayerD;
        cButton btnPlayerL;
        cButton btnBack;

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

            btnPlay = new cButton(Content.Load<Texture2D>("PlayButton"), graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width / 4, graphics.GraphicsDevice.Viewport.Height / 30);
            btnPlay.setPosition(new Vector2(100, 500));
            btnExit = new cButton(Content.Load<Texture2D>("ExitButton"), graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width / 4, graphics.GraphicsDevice.Viewport.Height / 30);
            btnExit.setPosition(new Vector2(500, 500));

            btnPlayerD = new cButton(Content.Load<Texture2D>("Deadpool 256"), graphics.GraphicsDevice, 256, 256);
            btnPlayerD.setPosition(new Vector2(100, 300));

            btnPlayerL = new cButton(Content.Load<Texture2D>("Link 256"), graphics.GraphicsDevice, 256, 256);
            btnPlayerL.setPosition(new Vector2(500, 300));

            _tileTexture = Content.Load<Texture2D>("tile");
            _jumperTexture = Content.Load<Texture2D>("jumper");
            jumper = new Jumper(_jumperTexture, new Vector2(50, 50), spriteBatch);
            board = new Board(spriteBatch, _tileTexture, 15, 10);

            debugFont = Content.Load<SpriteFont>("DebugFont");
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
                    if (btnPlay.isCLicked) CurrentGameState = GameState.SelectPlayer;
                    if (btnExit.isCLicked) this.Exit();
                    btnPlay.Update(mouse);
                    btnExit.Update(mouse);
                    break;
                case GameState.SelectPlayer:
                    if (btnPlayerD.isCLicked || btnPlayerL.isCLicked) CurrentGameState = GameState.Playing;
                    btnPlayerD.Update(mouse);
                    btnPlayerL.Update(mouse);
                    break;
                case GameState.Playing:
                    jumper.Update(gameTime);
                    break;
            }
            // TODO: Add your update logic here

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
                    btnPlayerL.Draw(spriteBatch);
                    break;
                case GameState.Playing:
                    string positionInText = string.Format("Position of Jumper: ({0:0.0}, {1:0.0})", jumper.Position.X, jumper.Position.Y);
                    string movementInText = string.Format("Current movement: ({0:0.0}, {1:0.0})", jumper.Movement.X, jumper.Movement.Y);
                    GraphicsDevice.Clear(Color.WhiteSmoke);
                    base.Draw(gameTime);
                    board.Draw();
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
