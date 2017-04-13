using FallingWorld;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Jumper : Sprite
{
    public Vector2 Movement { get; set; }
    private Vector2 oldPosition;
    public Texture2D TextureGauche { get; set; }
    public Texture2D TextureDroite { get; set; }
    public Texture2D TextureTete { get; set; }
    public int Score { get; set; }
    public int DoubleJump { get; set; }
    private KeyboardState oldState, newState;
    public int NbLife;

    public Jumper(Texture2D textureGauche, Texture2D textureDroite, Texture2D textureHead, Vector2 position, SpriteBatch spritebatch)
        : base(textureGauche, position, spritebatch)
    {
        this.TextureTete = textureHead;
        this.TextureDroite = textureDroite;
        this.TextureGauche = textureGauche;
        oldState = Keyboard.GetState();
        NbLife = 3;
        Score = 0;
    }


    public void Update(GameTime gameTime)
    {
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
        KeyboardState keyboardState = Keyboard.GetState();
        if (IsOnFirmGround()) { DoubleJump = 0; }

        if (keyboardState.IsKeyDown(Keys.Left)) { Movement += new Vector2(-1, 0); Texture = TextureGauche; }
        if (keyboardState.IsKeyDown(Keys.Right)) { Movement += new Vector2(1, 0); Texture = TextureDroite; }
        if (keyboardState.IsKeyDown(Keys.Up) && ((IsOnFirmGround() || DoubleJump < 2 ) && (newState.IsKeyDown(Keys.Up) && !oldState.IsKeyDown(Keys.Up))))
        {
            Movement = new Vector2(0, -1.0f) * 25;
            Score++;
            DoubleJump++;
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
        Rectangle onePixelLower = Bounds;
        onePixelLower.Offset(0, 1);
        return !Board.CurrentBoard.HasRoomForRectangle(onePixelLower);
    }

    private void StopMovingIfBlocked()
    {
        Vector2 lastMovement = Position - oldPosition;
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