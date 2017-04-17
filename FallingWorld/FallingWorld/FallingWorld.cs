using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace FallingWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FallingWorld : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        /*GifAnimation.GifAnimation anim;
        anim = Content.Load<GifAnimation.GifAnimation>("anim1");
        anim.Update(gameTime.ElapsedGameTime.Ticks);*/

        private Texture2D mainMenuTexture, selectPlayerTexture, selectWorldTexture, playingTexture;
        private Jumper jumper;
        private Vector2 initalPosition;

        private Board board;

        private SpriteFont debugFont, pixelFont;

        enum GameState
        {
            MainMenu,
            SelectPlayer,
            SelectWorld,
            Playing,
            Pause,
            GameOver,
        }

        GameState CurrentGameState = GameState.MainMenu;
        int screenWidth = 960;
        int screenHeight = 640;

        //Game button
        Button btnPlay, btnExit, btnResumePaused, btnExitPlay, btnReplay;

        //Player button
        Button btnPlayerD, btnPlayerS, btnPlayerP, btnPlayerW, btnPlayerI, btnPlayerC;
        String playerSelected;

        //World button
        Button btnWorld1, btnWorld2;

        //Pause and game over items
        Rectangle pausedRectangle;
        Texture2D pauseTexture;
        Texture2D gameOverTexture;
        SoundEffect gameOverSound;
        Stopwatch sw;

        //Game items
        String time, score, life, best;
        List<int> bestScores;
        int bestScore, loadedScore;
        int levelSelected;
        Rectangle newRecordRectangle;
        Texture2D newRecordTexture;
        Boolean isNewRecord;
        SoundEffect newRecordSong;

        //State of keybord before and after the update
        KeyboardState oldState, newState;

        //Object
        private Random random;
        public List<Object> objects { get; set; }
        public List<Object> objectsToRemove { get; set; }
        Object objectToSpawn;
        private int nbObjectSpawn;
        SoundEffect objectSong;

        //Meteor
        int randomPosX;
        int randomPosMvX;
        int randomPosMvY;

        public List<Meteor> meteors { get; set; }
        public List<Meteor> meteorsToRemove { get; set; }
        private int nbMeteor;
        private int nbMeteorSpawned;
        Stopwatch swMetor;
        Texture2D meteorTexture;
        SoundEffect meteorExplosionSong;

        List<int> availableRandomValue;
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
            mainMenuTexture = Content.Load<Texture2D>("MainMenu");
            selectPlayerTexture = Content.Load<Texture2D>("SelectPlayer");
            selectWorldTexture = Content.Load<Texture2D>("SelectWorld");
            playingTexture = Content.Load<Texture2D>("PlayBackground");

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

            //Init world button
            btnWorld1 = new Button(Content.Load<Texture2D>("World1"), graphics.GraphicsDevice, 256, 170, new Vector2(30, 120));
            btnWorld2 = new Button(Content.Load<Texture2D>("World2"), graphics.GraphicsDevice, 256, 170, new Vector2(300, 120));

            //Init pause/game over items
            pausedRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            pauseTexture = Content.Load<Texture2D>("Pause");
            gameOverTexture = Content.Load<Texture2D>("GameOver");
            gameOverSound = Content.Load<SoundEffect>("GameOverSong");
            
            board = new Board(spriteBatch, Content.Load<Texture2D>("Plateform"), 15, 20);

            debugFont = Content.Load<SpriteFont>("DebugFont");
            pixelFont = Content.Load<SpriteFont>("Pixel");
            bestScores = new List<int>();
            loadBestScore();
            objects = new List<Object>();
            objectsToRemove = new List<Object>();

            //Meteor
            meteors = new List<Meteor>();
            meteorsToRemove = new List<Meteor>();
            nbMeteor = 10;
            nbMeteorSpawned = 0;
            swMetor = new Stopwatch();
            meteorTexture = Content.Load<Texture2D>("Meteor");
            meteorExplosionSong = Content.Load<SoundEffect>("Explosion");
            objectSong = Content.Load<SoundEffect>("Object");

            newRecordSong = Content.Load<SoundEffect>("BeatBestScore");
            isNewRecord = false;
            newRecordRectangle = new Rectangle(268, 420, 423,83);
            newRecordTexture = Content.Load<Texture2D>("NewRecord");

            availableRandomValue = new List<int>();
            for(int i = -5; i < 6; ++i)
            {
                if (i != 0)
                    availableRandomValue.Add(i);
            }

            random = new Random();
            oldState = Keyboard.GetState();
            time = "TIME";
            score = "SCORE";
            life = "LIFE";
            best = "BEST";
            nbObjectSpawn = 0;
            playerSelected = "";
            initalPosition = new Vector2(460, 525);
            levelSelected = -1;
        }

        protected override void UnloadContent()
        {
           
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                prepareExit();

            //Get mouse information to fade in/out of button
            mouse = Mouse.GetState();

            //get current state of keybord to set pause
            newState = Keyboard.GetState();
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    if (btnPlay.isCLicked)
                    {
                        //Usefull to block click chaining
                        Thread.Sleep(1000);

                        resetClickButton();

                        jumper = new Jumper(null, null, null, 
                            Content.Load<Texture2D>("PlayerTransparent"), initalPosition, spriteBatch, null, null, null);
                        objects.Clear();
                        meteors.Clear();
                        CurrentGameState = GameState.SelectPlayer;
                    }

                    if (btnExit.isCLicked)
                        prepareExit();

                    btnPlay.Update(mouse);
                    btnExit.Update(mouse);
                    break;

                case GameState.SelectPlayer:

                    //Test for each button to know which player is selected
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

                    //If a player is selected, init the jumper object and go to world selection
                    if (!playerSelected.Equals(""))
                        initJumper(playerSelected);
                    

                    btnPlayerW.Update(mouse);
                    btnPlayerP.Update(mouse);
                    btnPlayerD.Update(mouse);
                    btnPlayerS.Update(mouse);
                    btnPlayerI.Update(mouse);
                    btnPlayerC.Update(mouse);
                    break;

                case GameState.SelectWorld:

                    //levelSelected = random.Next(0, 2);
                    if (btnWorld1.isCLicked)
                        levelSelected = 0;

                    if (btnWorld2.isCLicked)
                        levelSelected = 1;

                    if (levelSelected != -1)
                    {
                        board.grille = board.allGrille[levelSelected];
                        initGame();
                    }

                    btnWorld1.Update(mouse);
                    btnWorld2.Update(mouse);
                    break;
                case GameState.Playing:
                    
                    //Object generation
                    if(((int)sw.Elapsed.TotalMilliseconds / (10*1000)) > nbObjectSpawn)
                    {
                        int i = random.Next(0, 101);
                        if(i <= 60)
                            objectToSpawn = new Object(jumper.TextureObject100, 100, board.allAvailableSpawnPosition[levelSelected][random.Next(0, 2)], 50, 50, this, objectSong);
                        else if(i <= 90)
                            objectToSpawn = new Object(jumper.TextureObject500, 500, board.allAvailableSpawnPosition[levelSelected][random.Next(3, 4)], 50, 50, this, objectSong);
                        else
                            objectToSpawn = new Object(jumper.TextureObject1000, 1000, board.allAvailableSpawnPosition[levelSelected][5], 50, 50, this, objectSong);
                        objects.Add(objectToSpawn);
                        nbObjectSpawn++;
                    }

                    //Meteor generation
                    if(meteors.Count < nbMeteor && (int)swMetor.Elapsed.TotalMilliseconds/500 > nbMeteorSpawned)
                    {
                        randomPosX = random.Next(100, 860);

                        //Meteor spawned at the left of the screen, movement to right with a random speed
                        if (randomPosX < 420)
                            randomPosMvX = random.Next(1, 6);
                        //Meteor spawned at the right of the screen, movement to left with a random speed
                        else if (randomPosX > 540)
                            randomPosMvX = random.Next(-5, 0);
                        //Meteor spawned near the center of the screen, random movement and speed
                        else
                            randomPosMvX = availableRandomValue.ElementAt(random.Next(0, 9));

                        randomPosMvY = random.Next(1, 3);
                        meteors.Add(new Meteor(meteorTexture, new Vector2(randomPosX, -1), 50, 50, this, 
                            new Vector2(randomPosMvX, randomPosMvY), meteorExplosionSong));
                        nbMeteorSpawned++;
                    }

                    //Set pause only if in previous state the Escape button is not pressed
                    if (newState.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape))
                    {
                        sw.Stop();
                        swMetor.Stop();
                        btnResumePaused.isCLicked = false;
                        CurrentGameState = GameState.Pause;
                    }

                        jumper.Update(gameTime, sw);
                        foreach(Object o in objects)
                            o.Update(jumper, sw);
                        foreach(Object o in objectsToRemove)
                            objects.Remove(o);
                        objectsToRemove.Clear();

                        foreach (Meteor m in meteors)
                            m.Update(jumper);
                        foreach (Meteor m in meteorsToRemove)
                            meteors.Remove(m);
                        meteorsToRemove.Clear();

                    if (jumper.NbLife == 0)
                    {
                        btnReplay.isCLicked = false;
                        CurrentGameState = GameState.GameOver;
                        if(bestScore >= jumper.Score)
                            gameOverSound.Play(0.5f, 0f, 0f);
                    }
                    break;

                case GameState.Pause:
                    if (btnResumePaused.isCLicked || (newState.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape)))
                    {
                        resetClickButton();
                        sw.Start();
                        swMetor.Start();
                        CurrentGameState = GameState.Playing;
                    }

                    if (btnExitPlay.isCLicked)
                    {
                        levelSelected = -1;
                        resetClickButton();
                        playerSelected = "";
                        jumper.Score = 0;
                        objects.Clear();
                        CurrentGameState = GameState.MainMenu;
                    }

                    btnResumePaused.Update(mouse);
                    btnExitPlay.Update(mouse);
                    break;

                case GameState.GameOver:
                    if (jumper.Score > bestScore)
                    {
                        bestScore = jumper.Score;
                        bestScores[levelSelected] = jumper.Score;
                        newRecordSong.Play(0.5f, 0f, 0f);
                        isNewRecord = true;
                    }

                    if (sw.IsRunning)
                        sw.Stop();

                    //Replay
                    if (btnReplay.isCLicked)
                    {
                        jumper.Position = initalPosition;
                        jumper.Movement = new Vector2(0, 0);
                        jumper.Score = 0;
                        objects.Clear();
                        nbObjectSpawn = 0;
                        nbMeteorSpawned = 0;
                        meteors.Clear();
                        jumper.NbLife = 3;
                        resetClickButton();
                        sw.Restart();
                        isNewRecord = false;
                        CurrentGameState = GameState.Playing;
                    }

                    //Exit the playing state
                    if (btnExitPlay.isCLicked)
                    {
                        levelSelected = -1;
                        playerSelected = "";
                        resetClickButton();
                        isNewRecord = false;
                        CurrentGameState = GameState.MainMenu;
                    }

                    btnReplay.Update(mouse);
                    btnExitPlay.Update(mouse);
                    break;
            }
            oldState = newState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(mainMenuTexture, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    btnPlay.Draw(spriteBatch);
                    btnExit.Draw(spriteBatch);
                    break;
                case GameState.SelectPlayer:
                    spriteBatch.Draw(selectPlayerTexture, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    btnPlayerD.Draw(spriteBatch);
                    btnPlayerS.Draw(spriteBatch);
                    btnPlayerP.Draw(spriteBatch);
                    btnPlayerW.Draw(spriteBatch);
                    btnPlayerI.Draw(spriteBatch);
                    btnPlayerC.Draw(spriteBatch);
                    break;
                case GameState.SelectWorld:
                    spriteBatch.Draw(selectWorldTexture, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    btnWorld1.Draw(spriteBatch);
                    btnWorld2.Draw(spriteBatch);
                    break;
                case GameState.Playing:
                    drawGame(spriteBatch, gameTime);
                    break;
                case GameState.Pause:
                    drawGame(spriteBatch, gameTime);
                    spriteBatch.Draw(pauseTexture, pausedRectangle, Color.White);
                    btnExitPlay.Draw(spriteBatch);
                    btnResumePaused.Draw(spriteBatch);
                    break;
                case GameState.GameOver:
                    drawGame(spriteBatch, gameTime);
                    spriteBatch.Draw(gameOverTexture, pausedRectangle, Color.White);
                    btnExitPlay.Draw(spriteBatch);
                    btnReplay.Draw(spriteBatch);
                    if (isNewRecord)
                        spriteBatch.Draw(newRecordTexture, newRecordRectangle, Color.White);
                    break;
            }

            base.Draw(gameTime);
            spriteBatch.End();
        }


        private void drawGame(SpriteBatch spriteBatch, GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);
            spriteBatch.Draw(playingTexture, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            base.Draw(gameTime);
            board.Draw();

            foreach (Object o in objects)
            {
                o.Draw(spriteBatch);
            }

            foreach (Meteor m in meteors)
            {
                m.Draw(spriteBatch);
            }

            //Draw all game string (score, life, time)
            spriteBatch.DrawString(pixelFont, time, new Vector2(30, 0), Color.White);
            spriteBatch.DrawString(pixelFont, score, new Vector2(865, 0), Color.White);
            spriteBatch.DrawString(pixelFont, string.Format("{0:00}:{1:00}:{2:00}", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds), new Vector2(10, 20), Color.White);
            spriteBatch.DrawString(pixelFont, string.Format("{0:0000000}", jumper.Score), new Vector2(850, 20), Color.White);
            spriteBatch.DrawString(pixelFont, best, new Vector2(870, 40), Color.White);
            spriteBatch.DrawString(pixelFont, string.Format("{0:0000000}", bestScore < jumper.Score?jumper.Score:bestScore), new Vector2(850, 60), Color.White);
            spriteBatch.DrawString(pixelFont, life, new Vector2(460, 0), Color.White);
            int position = 394;
            for (int i = 0; i < jumper.NbLife; ++i)
            {
                spriteBatch.Draw(jumper.TextureTete, new Vector2(position, 25), Color.White);
                position += 60;
            }
            //spriteBatch.DrawString(debugFont, string.Format("Current movement: ({0:0.0}, {1:0.0})", jumper.Movement.X, jumper.Movement.Y), new Vector2(10, 40), Color.Black);
            //spriteBatch.DrawString(debugFont, string.Format("Value of doubleJump flag " + jumper.DoubleJump), new Vector2(10, 60), Color.Black);
            //spriteBatch.DrawString(debugFont, string.Format("Position of Jumper: ({0:0.0}, {1:0.0})", jumper.Position.X, jumper.Position.Y), new Vector2(10, 80), Color.Black);

            if (jumper.NbLife != 0)
                jumper.Draw();
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
            btnWorld1.isCLicked = false;
            btnWorld2.isCLicked = false;
        }

        //Load all jumper texture
        private void initJumper(String jumperName)
        {
            jumper.TextureTete = Content.Load<Texture2D>(jumperName + "Head");
            jumper.Texture = Content.Load<Texture2D>(jumperName + "Little");
            jumper.TextureGauche = Content.Load<Texture2D>(jumperName + "Little");
            jumper.TextureDroite = Content.Load<Texture2D>(jumperName + "LittleDroite");
            jumper.TextureObject100 = Content.Load<Texture2D>("DeadpoolItem100");
            jumper.TextureObject500 = Content.Load<Texture2D>("DeadpoolItem500");
            jumper.TextureObject1000 = Content.Load<Texture2D>("DeadpoolItem1000");
            resetClickButton();

            Thread.Sleep(1000);
            CurrentGameState = GameState.SelectWorld;
        }

        //Reset all game attribute
        private void initGame()
        {
            bestScore = bestScores[levelSelected];
            nbMeteorSpawned = 0;
            nbObjectSpawn = 0;
            sw = Stopwatch.StartNew();
            swMetor = Stopwatch.StartNew();
            isNewRecord = false;
            CurrentGameState = GameState.Playing;
        }

        //Export new score
        private void prepareExit()
        {
            XmlDocument doc = new XmlDocument();
            String pathToExe = System.Reflection.Assembly.GetEntryAssembly().Location;
            String path = pathToExe.Split(new string[] { "\\FallingWorld.exe" }, 3, StringSplitOptions.None)[0];

            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement element;
            element = doc.CreateElement(string.Empty, "body", string.Empty);
            int scoreCount = 0;
            XmlElement score;
            foreach (Tile[,] grille in board.allGrille)
            {
                score = doc.CreateElement(string.Empty, "bestScore" + scoreCount, string.Empty);
                score.InnerText = bestScores[scoreCount].ToString();
                element.AppendChild(score);
                scoreCount++;
            }
            doc.AppendChild(element);

            doc.Save(path + "\\config.xml");
            FileEncrypt.EncryptFile(path + "\\config.xml", path + "\\FallingWorld.xml");
            this.Exit();
        }

        //Save score in case of new record
        private void loadBestScore()
        {
            XmlDocument doc = new XmlDocument();
            String pathToExe = System.Reflection.Assembly.GetEntryAssembly().Location;
            String path = pathToExe.Split(new string[] { "\\FallingWorld.exe" }, 3, StringSplitOptions.None)[0] ;
            if (!File.Exists(path + "\\FallingWorld.xml"))
            {
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlElement element;
                element = doc.CreateElement(string.Empty, "body", string.Empty);
                int scoreCount = 0;
                XmlElement score;
                foreach (Tile[,] grille in board.allGrille)
                {
                    score = doc.CreateElement(string.Empty, "bestScore" + scoreCount, string.Empty);
                    score.InnerText = 0.ToString();
                    element.AppendChild(score);
                    scoreCount++;
                    bestScores.Add(0);
                }
                doc.AppendChild(element);

                doc.Save(path + "\\config.xml");
                FileEncrypt.EncryptFile(path +"\\config.xml", path + "\\FallingWorld.xml");
            }
            else
            {
                try
                {
                    FileEncrypt.DecryptFile(path + "\\FallingWorld.xml", path + "\\config.xml");
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(path + "\\config.xml");

                    int scoreCount = 0;
                    foreach (Tile[,] grille in board.allGrille)
                    {
                        bestScores.Add(Int16.Parse(xDoc.ChildNodes[1].ChildNodes[scoreCount].InnerText));
                        scoreCount++;
                    }
                    //File.Delete(path + "\\config.xml");
                }
                catch(Exception e)
                {
                    foreach (Tile[,] grille in board.allGrille)
                        bestScores.Add(0);
                }
            }
        }
    }
}
