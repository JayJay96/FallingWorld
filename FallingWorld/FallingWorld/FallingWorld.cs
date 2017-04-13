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
using System.Diagnostics;

namespace FallingWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FallingWorld : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Jumper jumper;

        private Board board;

        private SpriteFont debugFont, pixelFont;

        enum GameState
        {
            MainMenu,
            SelectPlayer,
            Playing
        }

        GameState CurrentGameState = GameState.MainMenu;
        int screenWidth = 960;
        int screenHeight = 640;

        //Game button
        Button btnPlay, btnExit, btnResumePaused, btnExitPlay, btnReplay;

        //Player button
        Button btnPlayerD, btnPlayerS, btnPlayerP, btnPlayerW, btnPlayerI, btnPlayerC;
        String playerSelected;

        //Pause and game over items
        Boolean paused = false;
        Rectangle pausedRectangle;
        Texture2D pauseTexture;
        Texture2D gameOverTexture;
        Stopwatch sw;

        //Game string
        String time, score, point, life;

        //State of keybord before and after the update
        KeyboardState oldState, newState;

        private Random random;
        private List<Vector2> objectPosition;
        public List<Object> objects { get; set; }
        public List<Object> objectsToRemove { get; set; }
        Object objectToSpawn;
        private int nbObjectSpawn;

        MouseState mouse;

        public FallingWorld()
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

            //Init game button
            btnPlay = new Button(Content.Load<Texture2D>("PlayButtonTransparent"), graphics.GraphicsDevice, 200, 50, new Vector2(100, 500));
            btnExit = new Button(Content.Load<Texture2D>("QuitButton"), graphics.GraphicsDevice, 200, 50, new Vector2(660, 500));

            btnResumePaused = new Button(Content.Load<Texture2D>("ResumeButton"), graphics.GraphicsDevice, 200, 50, new Vector2(375, 240));
            btnReplay = new Button(Content.Load<Texture2D>("ReplayButton"), graphics.GraphicsDevice, 200, 50, new Vector2(375, 240));
            btnExitPlay = new Button(Content.Load<Texture2D>("MenuButton"), graphics.GraphicsDevice, 200, 50, new Vector2(375, 300));

            //Init player button
            btnPlayerD = new Button(Content.Load<Texture2D>("Deadpool 256"), graphics.GraphicsDevice, 256, 256, new Vector2(100, 330));
            btnPlayerS = new Button(Content.Load<Texture2D>("Spiderman 256"), graphics.GraphicsDevice, 256, 256, new Vector2(350, 330));
            btnPlayerP = new Button(Content.Load<Texture2D>("BlackPanthere 256"), graphics.GraphicsDevice, 256, 256, new Vector2(600, 330));
            btnPlayerW = new Button(Content.Load<Texture2D>("WarMachine 256"), graphics.GraphicsDevice, 256, 256, new Vector2(100, 80));
            btnPlayerI = new Button(Content.Load<Texture2D>("IronMan 256"), graphics.GraphicsDevice, 256, 256, new Vector2(350, 80));
            btnPlayerC = new Button(Content.Load<Texture2D>("CaptainAmerica 256"), graphics.GraphicsDevice, 256, 256, new Vector2(600, 80));

            //Init pause/game over items
            pausedRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            pauseTexture = Content.Load<Texture2D>("Pause");
            gameOverTexture = Content.Load<Texture2D>("GameOver");
            
            board = new Board(spriteBatch, Content.Load<Texture2D>("Plateform"), Content.Load<Texture2D>("PlateformLeft"), Content.Load<Texture2D>("PlateformRight"), 15, 20);

            debugFont = Content.Load<SpriteFont>("DebugFont");
            pixelFont = Content.Load<SpriteFont>("Pixel");

            objects = new List<Object>();
            objectsToRemove = new List<Object>();
            objectPosition = new List<Vector2>();
            objectPosition.Add(new Vector2(137, 400));
            objectPosition.Add(new Vector2(45, 200));
            objectPosition.Add(new Vector2(454, 300));
            objectPosition.Add(new Vector2(774, 400));
            objectPosition.Add(new Vector2(871, 200));
            objectPosition.Add(new Vector2(460, 100));
            random = new Random();
            /*objects.Add(new Object(Content.Load<Texture2D>("DeadpoolHead"), 500, , 50, 50, this));
            objects.Add(new Object(Content.Load<Texture2D>("IronManHead"), 500, , 50, 50, this));
            objects.Add(new Object(Content.Load<Texture2D>("BlackPanthereHead"), 500, new Vector2(454, 300), 50, 50, this));
            objects.Add(new Object(Content.Load<Texture2D>("WarMachineHead"), 500, new Vector2(774, 400), 50, 50, this));
            objects.Add(new Object(Content.Load<Texture2D>("SpidermanHead"), 500, new Vector2(871, 200), 50, 50, this));
            objects.Add(new Object(Content.Load<Texture2D>("CaptainAmericaHead"), 500, new Vector2(460, 100), 50, 50, this));*/
            oldState = Keyboard.GetState();
            time = "TIME";
            score = "SCORE";
            life = "LIFE";
            point = "0000000";
            nbObjectSpawn = 0;
            playerSelected = "";
        }

        protected override void UnloadContent()
        {
           
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            mouse = Mouse.GetState();

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    if (btnPlay.isCLicked)
                    {
                        Thread.Sleep(1500);
                        resetClickButton();
                        jumper = new Jumper(null, null, null, new Vector2(460, 525), spriteBatch);
                        CurrentGameState = GameState.SelectPlayer;
                    }

                    if (btnExit.isCLicked)
                        this.Exit();

                    btnPlay.Update(mouse);
                    btnExit.Update(mouse);
                    break;

                case GameState.SelectPlayer:
                    if (btnPlayerD.isCLicked)
                        playerSelected = "Deadpool";

                    if (btnPlayerS.isCLicked)
                        playerSelected = "Spiderman";

                    if (btnPlayerP.isCLicked)
                        playerSelected = "BlackPanthere";

                    if (btnPlayerW.isCLicked)
                        playerSelected = "WarMachine";

                    if (btnPlayerI.isCLicked)
                        playerSelected = "IronMan";

                    if (btnPlayerC.isCLicked)
                        playerSelected = "CaptainAmerica";

                    if (!playerSelected.Equals(""))
                        initJumper(playerSelected);
                    

                    btnPlayerW.Update(mouse);
                    btnPlayerP.Update(mouse);
                    btnPlayerD.Update(mouse);
                    btnPlayerS.Update(mouse);
                    btnPlayerI.Update(mouse);
                    btnPlayerC.Update(mouse);
                    break;

                case GameState.Playing:
                    newState = Keyboard.GetState();
                    if (jumper.NbLife > 0)
                    {
                        if (!paused)
                        {
                            if(((int)sw.Elapsed.TotalMilliseconds / (20*1000)) > nbObjectSpawn)
                            {
                                int i = random.Next(0, 101);
                                if(i <= 60)
                                    objectToSpawn = new Object(jumper.TextureTete, 100, objectPosition[random.Next(0, 5)], 50, 50, this);
                                else if(i <= 90)
                                    objectToSpawn = new Object(jumper.TextureTete, 500, objectPosition[random.Next(0, 5)], 50, 50, this);
                                else
                                    objectToSpawn = new Object(jumper.TextureTete, 1000, objectPosition[random.Next(0, 5)], 50, 50, this);
                                objects.Add(objectToSpawn);
                                nbObjectSpawn++;
                            }

                            //Set pause only if in previous state the Escape button is not pressed
                            if (newState.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape))
                                paused = true;
                            jumper.Update(gameTime);
                            foreach(Object o in objects)
                            {
                                o.Update(jumper);
                            }
                            foreach(Object o in objectsToRemove)
                            {
                                objects.Remove(o);
                            }
                            objectsToRemove.Clear();
                        }
                        else
                        {
                            //Remove pause only if in previous state the Escape button is not pressed
                            if (btnResumePaused.isCLicked || (newState.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape)))
                            {
                                resetClickButton();
                                paused = false;
                            }

                            if (btnExitPlay.isCLicked)
                            {
                                resetClickButton();
                                playerSelected = "";
                                jumper.Score = 0;
                                objects.Clear();
                                paused = false;
                                CurrentGameState = GameState.MainMenu;
                            }
                            btnResumePaused.Update(mouse);
                            btnExitPlay.Update(mouse);
                        }
                    }
                    else
                    {
                        if (sw.IsRunning)
                            sw.Stop();
                        if (btnReplay.isCLicked)
                        {
                            jumper.Score = 0;
                            objects.Clear();
                            jumper.NbLife = 3;
                            resetClickButton();
                            sw.Restart();
                        }
                        if (btnExitPlay.isCLicked)
                        {
                            playerSelected = "";
                            resetClickButton();
                            CurrentGameState = GameState.MainMenu;
                        }
                        btnReplay.Update(mouse);
                        btnExitPlay.Update(mouse);
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
                    btnPlayerP.Draw(spriteBatch);
                    btnPlayerW.Draw(spriteBatch);
                    btnPlayerI.Draw(spriteBatch);
                    btnPlayerC.Draw(spriteBatch);
                    break;
                case GameState.Playing:
                    //spriteBatch.Draw(Content.Load<Texture2D>("PlayBackground"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    GraphicsDevice.Clear(Color.WhiteSmoke);
                    base.Draw(gameTime);
                    board.Draw();


                    spriteBatch.DrawString(pixelFont, time, new Vector2(30, 0), Color.Black);
                    spriteBatch.DrawString(pixelFont, score, new Vector2(865, 0), Color.Black);
                    spriteBatch.DrawString(pixelFont, string.Format("{0:00}:{1:00}:{2:00}", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds), new Vector2(10, 20), Color.Black);
                    spriteBatch.DrawString(pixelFont, string.Format("{0:0000000}", jumper.Score), new Vector2(850, 20), Color.Black);
                    spriteBatch.DrawString(pixelFont, life, new Vector2(460, 0), Color.Black);
                    int position = 394;
                    for (int i = 0; i < jumper.NbLife; ++i)
                    {
                        spriteBatch.Draw(jumper.TextureTete, new Vector2(position, 25), Color.White);
                        position += 60;
                    }
                    //spriteBatch.DrawString(debugFont, string.Format("Current movement: ({0:0.0}, {1:0.0})", jumper.Movement.X, jumper.Movement.Y), new Vector2(10, 40), Color.Black);
                    //spriteBatch.DrawString(debugFont, string.Format("Value of doubleJump flag " + jumper.DoubleJump), new Vector2(10, 60), Color.Black);
                    spriteBatch.DrawString(debugFont, string.Format("Position of Jumper: ({0:0.0}, {1:0.0})", jumper.Position.X, jumper.Position.Y), new Vector2(10, 80), Color.Black);
                    jumper.Draw();
                    foreach (Object o in objects)
                    {
                        o.Draw(spriteBatch);
                    }
                    if (paused)
                    {
                        spriteBatch.Draw(pauseTexture, pausedRectangle, Color.White);
                        btnExitPlay.Draw(spriteBatch);
                        btnResumePaused.Draw(spriteBatch);
                    }
                    if(jumper.NbLife == 0)
                    {
                        spriteBatch.Draw(gameOverTexture, pausedRectangle, Color.White);
                        btnExitPlay.Draw(spriteBatch);
                        btnReplay.Draw(spriteBatch);
                    }
                    break;
            }

            base.Draw(gameTime);
            spriteBatch.End();
        }

        //Security : unclick all button
        private void resetClickButton()
        {
            btnResumePaused.isCLicked = false;
            btnReplay.isCLicked = false;
            btnExitPlay.isCLicked = false;
            btnPlay.isCLicked = false;
            btnPlayerD.isCLicked = false;
            btnPlayerS.isCLicked = false;
            btnPlayerP.isCLicked = false;
            btnPlayerW.isCLicked = false;
            btnPlayerI.isCLicked = false;
            btnPlayerC.isCLicked = false;
            btnExit.isCLicked = false;
        }

        private void initJumper(String jumperName)
        {
            jumper.TextureTete = Content.Load<Texture2D>(jumperName + "Head");
            jumper.Texture = Content.Load<Texture2D>(jumperName + "Little");
            jumper.TextureGauche = Content.Load<Texture2D>(jumperName + "Little");
            jumper.TextureDroite = Content.Load<Texture2D>(jumperName + "LittleDroite");
            resetClickButton();
            sw = Stopwatch.StartNew();
            CurrentGameState = GameState.Playing;
        }
    }
}
