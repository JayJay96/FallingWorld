using FallingWorld;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Jumper : Sprite
{
    public Vector2 Movement { get; set; }

    public Jumper(Texture2D texture, Vector2 position, SpriteBatch spritebatch)
        : base(texture, position, spritebatch)
    {
    }


    public void Update(GameTime gameTime)
    {
        CheckKeyboardAndUpdateMovement();
        SimulateFriction();
        UpdatePositionBasedOnMovement(gameTime);
    }

    private void CheckKeyboardAndUpdateMovement()
    {
        KeyboardState keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Left)) { Movement += new Vector2(-1, 0); }
        if (keyboardState.IsKeyDown(Keys.Right)) { Movement += new Vector2(1, 0); }
        if (keyboardState.IsKeyDown(Keys.Up)) { Movement += new Vector2(0, -1); }
    }

    private void SimulateFriction()
    {
        Movement -= Movement * new Vector2(.1f, .1f);
    }

    private void UpdatePositionBasedOnMovement(GameTime gameTime)
    {
        Position += Movement * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 15;
    }
}