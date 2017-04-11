using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FallingWorld
{
    class Board
    {
        private Tile[,] grille { get; set; }
        private int columns { get; set; }
        private int rows { get; set; }
        private Texture2D texture { get; set; }
        private SpriteBatch spriteBatch { get; set; }

        private Random _rnd = new Random();

        public Board(SpriteBatch spritebatch, Texture2D tileTexture, int columns, int rows)
        {
            this.columns = columns;
            this.rows = rows;
            texture = tileTexture;
            this.spriteBatch = spritebatch;
            InitializeAllTilesAndBlockSomeRandomly();
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Vector2 tilePosition =
                        new Vector2(x * tileTexture.Width, y * tileTexture.Height);
                    grille[x, y] =
                        new Tile(tileTexture, tilePosition, spritebatch, _rnd.Next(5) == 0);
                }
            }
            setFloorBlocked();

        }

        private void InitializeAllTilesAndBlockSomeRandomly()
        {
            grille = new Tile[columns, rows];
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Vector2 tilePosition =
                        new Vector2(x * texture.Width, y * texture.Height);
                    grille[x, y] =
                        new Tile(texture, tilePosition, spriteBatch, _rnd.Next(5) == 0);
                }
            }
        }

        private void setFloorBlocked()
        {
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (y == rows - 1)
                    { grille[x, y].IsBlocked = true; }
                }
            }
        }

        public void Draw()
        {
            foreach (var tile in grille)
            {
                tile.Draw();
            }
        }

    }
}
