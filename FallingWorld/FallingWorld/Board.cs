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
        private Texture2D leftPlateformTextuxe { get; set; }
        private Texture2D rightPlateformTextuxe { get; set; }
        private SpriteBatch spriteBatch { get; set; }

        public static Board CurrentBoard { get; private set; }

        private Random _rnd = new Random();

        public Board(SpriteBatch spritebatch, Texture2D tileTexture, Texture2D leftPlateformTextuxe, Texture2D rightPlateformTextuxe, int columns, int rows)
        {
            this.columns = columns;
            this.rows = rows;
            texture = tileTexture;
            this.leftPlateformTextuxe = leftPlateformTextuxe;
            this.rightPlateformTextuxe = rightPlateformTextuxe;
            this.spriteBatch = spritebatch;
            grille = new Tile[columns, rows];
            InitializeAllTilesAndBlockSomeRandomly();
            initGrid();
            Board.CurrentBoard = this;

        }

        private void InitializeAllTilesAndBlockSomeRandomly()
        {
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
               { 
                    Vector2 tilePosition =
                        new Vector2(x * texture.Width, y * texture.Height);
                    grille[x, y] =
                        new Tile(texture, tilePosition, spriteBatch,false);
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

        private void initGrid()
        {
            for(int x = 0; x < columns; ++x)
                grille[x, rows - 1].IsBlocked = true;

            for(int x = 1; x < 4; ++x)
                grille[x, 16].IsBlocked = true;

            for (int x = 11; x < 14; ++x)
                grille[x, 16].IsBlocked = true;

            for(int x = 5; x < 10; ++x)
                grille[x, 12].IsBlocked = true;

            grille[0, 9].Texture = leftPlateformTextuxe;
            grille[0, 9].IsBlocked = true;
            grille[1, 9].Texture = rightPlateformTextuxe;
            grille[1, 9].IsBlocked = true;

            grille[13, 9].Texture = leftPlateformTextuxe;
            grille[13, 9].IsBlocked = true;
            grille[14, 9].Texture = rightPlateformTextuxe;
            grille[14, 9].IsBlocked = true;

            grille[3, 6].Texture = leftPlateformTextuxe;
            grille[3, 6].IsBlocked = true;
            grille[11, 6].Texture = rightPlateformTextuxe;
            grille[11, 6].IsBlocked = true;

            for (int i = 4; i < 11; ++i)
                grille[i, 6].IsBlocked = true;

            grille[0, rows - 1].Texture = leftPlateformTextuxe;
            grille[columns -1, rows - 1].Texture = rightPlateformTextuxe;

            grille[1, 16].Texture = leftPlateformTextuxe;
            grille[11, 16].Texture = leftPlateformTextuxe;
            grille[5, 12].Texture = leftPlateformTextuxe;

            grille[3, 16].Texture = rightPlateformTextuxe;
            grille[13, 16].Texture = rightPlateformTextuxe;
            grille[9, 12].Texture = rightPlateformTextuxe;
        }

        public void Draw()
        {
            foreach (var tile in grille)
            {
                tile.Draw();
            }
        }

        public bool HasRoomForRectangle(Rectangle rectangleToCheck)
        {
            foreach (var tile in grille)
            {
                if (tile.IsBlocked && tile.Bounds.Intersects(rectangleToCheck))
                {
                    return false;
                }
            }
            return true;
        }

        public Vector2 WhereCanIGetTo(Vector2 originalPosition, Vector2 destination, Rectangle boundingRectangle)
        {
           Vector2 movementToTry = destination - originalPosition;
    Vector2 furthestAvailableLocationSoFar = originalPosition;
    int numberOfStepsToBreakMovementInto = (int)(movementToTry.Length() * 2) + 1;
    Vector2 oneStep = movementToTry / numberOfStepsToBreakMovementInto;
 
    for (int i = 1; i <= numberOfStepsToBreakMovementInto; i++)
    {
        Vector2 positionToTry = originalPosition + oneStep * i;
        Rectangle newBoundary = CreateRectangleAtPosition(positionToTry, boundingRectangle.Width, boundingRectangle.Height);
        if (HasRoomForRectangle(newBoundary)) { furthestAvailableLocationSoFar = positionToTry; }
        else
        {
            bool isDiagonalMove = movementToTry.X != 0 && movementToTry.Y != 0;
            if (isDiagonalMove)
            {
                int stepsLeft = numberOfStepsToBreakMovementInto - (i - 1);
 
                Vector2 remainingHorizontalMovement = oneStep.X * Vector2.UnitX * stepsLeft;
                Vector2 finalPositionIfMovingHorizontally = furthestAvailableLocationSoFar + remainingHorizontalMovement;
                furthestAvailableLocationSoFar =
                    WhereCanIGetTo(furthestAvailableLocationSoFar, finalPositionIfMovingHorizontally, boundingRectangle);
 
                Vector2 remainingVerticalMovement = oneStep.Y * Vector2.UnitY * stepsLeft;
                Vector2 finalPositionIfMovingVertically = furthestAvailableLocationSoFar + remainingVerticalMovement;
                furthestAvailableLocationSoFar =
                    WhereCanIGetTo(furthestAvailableLocationSoFar, finalPositionIfMovingVertically, boundingRectangle);
            }
            break;
        }
    }
    return furthestAvailableLocationSoFar;
        }

        private Rectangle CreateRectangleAtPosition(Vector2 positionToTry, int width, int height)
        {
            return new Rectangle((int)positionToTry.X, (int)positionToTry.Y, width, height);
        }

    }
}
