using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FallingWorld
{
    public class Meteor
    {
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;
        Vector2 movement;
        Vector2 size;
        FallingWorld game;
        SoundEffect explosionSong;
        Color color = new Color(255, 255, 255, 255);

        public Meteor(Texture2D texture, Vector2 position, float sizeX, float sizeY, FallingWorld game, Vector2 movement, SoundEffect explosionSong)
        {
            this.size = new Vector2(sizeX, sizeY);
            this.position = position;
            this.texture = texture;
            this.game = game;
            this.movement = movement;
            this.explosionSong = explosionSong;
        }

        public void Update(Jumper jumper)
        {
            if (position.Y > 580 || position.X < -10 || position.X > 970)
            {
                game.meteorsToRemove.Add(this);
            }
            else
            {
                rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

                Rectangle jumperRectangle = new Rectangle((int)jumper.Position.X, (int)jumper.Position.Y, 47, 47);

                if (jumperRectangle.Intersects(rectangle))
                {
                    if (!jumper.swHit.IsRunning)
                    {
                        jumper.NbLife--;
                        game.meteorsToRemove.Add(this);
                        if(jumper.NbLife != 0)
                            explosionSong.Play(0.5f, 0f, 0f);
                        jumper.swHit = Stopwatch.StartNew();
                    }
                }
                position += movement;
            }
        }

        public void Draw(SpriteBatch spirtBatch)
        {
            spirtBatch.Draw(texture, rectangle, Color.White);
        }
    }
}
