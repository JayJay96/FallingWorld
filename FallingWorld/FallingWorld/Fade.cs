using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FallingWorld
{
    public class Fade
    {
        Texture2D texture;
        Rectangle rectangle;
        Boolean fadeIn;
        public Boolean fadeFinished { get; set; }
        Color color;

        public Fade(Boolean fadeIn, int screenWidth, int screenHeight, Texture2D texture)
        {
            this.texture = texture;
            this.rectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            this.fadeIn = fadeIn;
            if(fadeIn)
                color = new Color(0, 0, 0, 0);
            else
                color = new Color(0, 0, 0, 255);
        }

        public void doFade()
        {

            if (fadeIn)
            {
                if (color.A - 2 >= 0)
                    color.A -= 2;
                else
                    fadeFinished = true;
            }
            else
            {
                if (color.A + 2 <= 255)
                    color.A += 2;
                else
                    fadeFinished = true;
            }
        }

        public void Draw(SpriteBatch spirtBatch)
        {
            spirtBatch.Draw(texture, rectangle, color);
        }
    }
}
