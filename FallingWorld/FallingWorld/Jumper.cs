using FallingWorld;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;


public class Jumper : Sprite
{
    public Vector2 Movement { get; set; }
    private Vector2 oldPosition;
    public Texture2D TextureTransp { get; set; }
    public Texture2D TextureGauche { get; set; }
    public Texture2D TextureDroite { get; set; }
    public Texture2D TextureTete { get; set; }
    public Texture2D TextureObject100 { get; set; }
    public Texture2D TextureObject500 { get; set; }
    public Texture2D TextureObject1000 { get; set; }
    private Texture2D oldTexture;
    public int Score { get; set; }
    public int DoubleJump { get; set; }
    private KeyboardState oldState, newState;
    public int NbLife;
    private Rectangle onePixelLower;
    private Vector2 lastMovement;
    private KeyboardState keyboardState;
    public Stopwatch swHit { get; set; }

    public Jumper(Texture2D textureGauche, Texture2D textureDroite, Texture2D textureHead, 
        Texture2D textureTransp, Vector2 position, SpriteBatch spritebatch, Texture2D textureObject100,
        Texture2D textureObject500, Texture2D textureObject1000)
        : base(textureGauche, position, spritebatch)
    {
        this.TextureTete = textureHead;
        this.TextureDroite = textureDroite;
        this.TextureGauche = textureGauche;
        this.TextureTransp = textureTransp;
        oldTexture = TextureGauche;
        oldState = Keyboard.GetState();
        NbLife = 3;
        Score = 0;
        swHit = new Stopwatch();
        this.TextureObject100 = textureObject100;
        this.TextureObject500 = textureObject500;
        this.TextureObject1000 = textureObject1000;
    }


    public void Update(GameTime gameTime, Stopwatch sw)
    {
        if (!sw.IsRunning)
            swHit.Stop();
        else if(swHit.Elapsed.TotalMilliseconds > 0)
            swHit.Start();

        if (swHit.IsRunning && swHit.Elapsed.Seconds > 2)
            swHit.Stop();

        newState = Keyboard.GetState();
        CheckKeyboardAndUpdateMovement();
        AffectWithGravity();
        SimulateFriction();
        MoveAsFarAsPossible(gameTime);
        StopMovingIfBlocked();
        CheckPosition();
        oldState = newState;
    }

    private void AffectWithGravity()
    {
        Movement += Vector2.UnitY * 1f;
    }

    private void MoveAsFarAsPossible(GameTime gameTime)
    {
        Vector2 oldPosition = Position;
        UpdatePositionBasedOnMovement(gameTime);
        Position = Board.CurrentBoard.WhereCanIGetTo(oldPosition, Position, Bounds);
    }

    private void CheckKeyboardAndUpdateMovement()
    {
        keyboardState = Keyboard.GetState();
        if (IsOnFirmGround()) { DoubleJump = 0; }

        if (keyboardState.IsKeyDown(Keys.Left))
        {
            Movement += new Vector2(-1, 0);
            oldTexture = TextureGauche;
            if (!swHit.IsRunning || (swHit.Elapsed.TotalMilliseconds > 250 && swHit.Elapsed.TotalMilliseconds < 500)
                || (swHit.Elapsed.TotalMilliseconds > 750 && swHit.Elapsed.TotalMilliseconds < 1000)
                || (swHit.Elapsed.TotalMilliseconds > 1250 && swHit.Elapsed.TotalMilliseconds < 1500)
                || (swHit.Elapsed.TotalMilliseconds > 1750 && swHit.Elapsed.TotalMilliseconds < 3000))
                Texture = TextureGauche;
            else
                Texture = TextureTransp;

        }
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            Movement += new Vector2(1, 0);
            oldTexture = TextureDroite;
            if (!swHit.IsRunning || (swHit.Elapsed.TotalMilliseconds > 250 && swHit.Elapsed.TotalMilliseconds < 500)
                || (swHit.Elapsed.TotalMilliseconds > 750 && swHit.Elapsed.TotalMilliseconds < 1000)
                || (swHit.Elapsed.TotalMilliseconds > 1250 && swHit.Elapsed.TotalMilliseconds < 1500)
                || (swHit.Elapsed.TotalMilliseconds > 1750 && swHit.Elapsed.TotalMilliseconds < 3000))
                Texture = TextureDroite;
            else
                Texture = TextureTransp;
        }

        if (keyboardState.IsKeyDown(Keys.Up) && ((IsOnFirmGround() || DoubleJump < 2) && (newState.IsKeyDown(Keys.Up) && !oldState.IsKeyDown(Keys.Up))))
        {
            Movement = new Vector2(0, -1.0f) * 25;
            DoubleJump++;
        }

        if (swHit.IsRunning && !keyboardState.IsKeyDown(Keys.Left) && !keyboardState.IsKeyDown(Keys.Right))
        {
            if ((swHit.Elapsed.TotalMilliseconds > 250 && swHit.Elapsed.TotalMilliseconds < 500)
                || (swHit.Elapsed.TotalMilliseconds > 750 && swHit.Elapsed.TotalMilliseconds < 1000)
                || (swHit.Elapsed.TotalMilliseconds > 1250 && swHit.Elapsed.TotalMilliseconds < 1500)
                || (swHit.Elapsed.TotalMilliseconds > 1750 && swHit.Elapsed.TotalMilliseconds < 3000))
                Texture = oldTexture;
            else
                Texture = TextureTransp;
        }
    }

    private void SimulateFriction()
    {
        Movement -= Movement * new Vector2(.1f, .1f);
    }

    private void UpdatePositionBasedOnMovement(GameTime gameTime)
    {
        Position += Movement * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 15;
    }

    public bool IsOnFirmGround()
    {
        onePixelLower = Bounds;
        onePixelLower.Offset(0, 1);
        return !Board.CurrentBoard.HasRoomForRectangle(onePixelLower);
    }

    private void StopMovingIfBlocked()
    {
        lastMovement = Position - oldPosition;
        if (lastMovement.X == 0) { Movement *= Vector2.UnitY; }
        if (lastMovement.Y == 0) { Movement *= Vector2.UnitX; }
    }

    private void CheckPosition()
    {
        if(Position.Y > 600)
        {
            NbLife--;
            Position = new Vector2(460, 525);
        }
    }
}