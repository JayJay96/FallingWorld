using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;


namespace FallingWorld
{
    public class Button
    {
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;

        bool increase;
        public bool isCLicked = false;
        public Boolean Intersect { get; set; }

        Color color = new Color(255, 255, 255, 255);
        float defaultX, defaultY, defaultPosX;

        public Vector2 size;

        public Button(Texture2D texture, GraphicsDevice graphics, float sizeX, float sizeY, Vector2 position, bool doIncrease)
        {
            this.Intersect = false;
            this.texture = texture;
            this.size = new Vector2(sizeX, sizeY);
            this.defaultX = sizeX;
            this.defaultY = sizeY;
            defaultPosX = position.X;
            this.position = position;
            increase = doIncrease;
        }

        public void Update(MouseState mouse)
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (mouseRectangle.Intersects(rectangle)){
                Intersect = true;
                if(mouse.LeftButton == ButtonState.Pressed) isCLicked = true;
                if (increase && position.X == defaultPosX)
                {
                    size = new Vector2(defaultX + defaultX * 0.15f, defaultY + defaultY * 0.15f);
                    position.X -= defaultX * 0.15f;
                    position.Y -= defaultY * 0.15f;
                }
            }

            else 
            {
                Intersect = false;

                if (increase && position.X < defaultPosX)
                {
                    size = new Vector2(defaultX, defaultY);
                    position.X += defaultX * 0.15f;
                    position.Y += defaultY * 0.15f;
                }
            }
        }

        public void setPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public void Draw(SpriteBatch spirtBatch)
        {
            spirtBatch.Draw(texture, rectangle, color);
        }
    }
}
