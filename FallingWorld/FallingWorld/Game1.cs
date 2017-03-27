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

        enum GameState
        {
            MainMenu,
            SelectPlayer,
            Playing
        }

        GameState CurrentGameState = GameState.MainMenu;
        int screenWidth = 800;
        int screenHeight = 600;

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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
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
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
                    btnPlayerD.Update(mouse);
                    btnPlayerL.Update(mouse);
                    break;
                case GameState.Playing:
                    
                    break;
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
                    break;
            }

            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
