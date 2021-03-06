﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FallingWorld
{
    public class Object
    {
        Rectangle rectangle;
        Texture2D texture;
        Vector2 position;
        Vector2 size;
        int scoreGive;
        FallingWorld game;
        Stopwatch swObject;
        SoundEffect objectSound;

        public Object(Texture2D texture, int scoreGive, Vector2 position, float sizeX, float sizeY, FallingWorld game, SoundEffect objectSound)
        {
            this.size = new Vector2(sizeX, sizeY);
            this.position = position;
            this.scoreGive = scoreGive;
            this.texture = texture;
            this.game = game;
            this.objectSound = objectSound;
            swObject = Stopwatch.StartNew();
        }


        public void Update(Jumper jumper, Stopwatch sw)
        {
            if (!sw.IsRunning)
                swObject.Stop();
            else
                swObject.Start();

            if ((int)swObject.Elapsed.TotalMilliseconds / 10000 > 0)
                game.objectsToRemove.Add(this);
            else
            {
                rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

                Rectangle jumperRectangle = new Rectangle((int)jumper.Position.X, (int)jumper.Position.Y, 47, 47);

                if (jumperRectangle.Intersects(rectangle))
                {
                    jumper.Score += this.scoreGive;
                    objectSound.Play(0.5f, 0f, 0f);
                    game.objectsToRemove.Add(this);
                }
            }
        }

        public void Draw(SpriteBatch spirtBatch)
        {
            spirtBatch.Draw(texture, rectangle, Color.White);
        }
    }
}
