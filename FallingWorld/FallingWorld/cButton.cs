using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FallingWorld
{
    class cButton
    {
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;

        Color color = new Color(255, 255, 255, 255);

        public Vector2 size;

        public cButton(Texture2D texture, GraphicsDevice graphics, float sizeX, float sizeY)
        {
            this.texture = texture;
            this.size = new Vector2(sizeX, sizeY);
        }

        bool down;
        public bool isCLicked;

        public void Update(MouseState mouse)
        {
            rectangle = new Rectangle((int)position.X, (int) position.Y, (int)size.X, (int)size.Y);

            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (mouseRectangle.Intersects(rectangle))
            {
                if (color.A == 255) down = false;
                else if (color.A == 0) down = true;

                if (down) color.A += 3;
                else color.A -= 3;

                if (mouse.LeftButton == ButtonState.Pressed) isCLicked = true;
            }
            else if(color.A < 255)
            {
                color.A += 3;
                isCLicked = false;
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
