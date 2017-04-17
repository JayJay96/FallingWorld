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
        public Tile[,] grille { get; set; }
        public List<Tile[,]> allGrille { get; set;}
        public List<List<Vector2>> allAvailableSpawnPosition { get; set; }
        private int columns { get; set; }
        private int rows { get; set; }
        private Texture2D texture { get; set; }
        private SpriteBatch spriteBatch { get; set; }

        public static Board CurrentBoard { get; private set; }

        private Random _rnd = new Random();

        public Board(SpriteBatch spritebatch, Texture2D tileTexture, int columns, int rows)
        {
            allAvailableSpawnPosition = new List<List<Vector2>>();
            allGrille = new List<Tile[,]>();
            this.columns = columns;
            this.rows = rows;
            texture = tileTexture;
            this.spriteBatch = spritebatch;
            grille = new Tile[columns, rows];
            InitializeAllTilesAndBlockSomeRandomly();
            initMap1();
            initMap2();
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

        private void InitializeAllTilesAndBlockSomeRandomly(Tile[,] grille)
        {
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Vector2 tilePosition =
                        new Vector2(x * texture.Width, y * texture.Height);
                    grille[x, y] =
                        new Tile(texture, tilePosition, spriteBatch, false);
                }
            }
        }

        private void setFloorBlocked(Tile[,] grille)
        {
            for (int x = 0; x < columns; ++x)
                grille[x, rows - 1].IsBlocked = true;
        }

        private void initMap1()
        {
            Tile[,] grille = new Tile[columns,rows];
            InitializeAllTilesAndBlockSomeRandomly(grille);
            setFloorBlocked(grille);

            for(int x = 1; x < 4; ++x)
                grille[x, 16].IsBlocked = true;

            for (int x = 11; x < 14; ++x)
                grille[x, 16].IsBlocked = true;

            for(int x = 5; x < 10; ++x)
                grille[x, 12].IsBlocked = true;

            grille[0, 9].IsBlocked = true;
            grille[1, 9].IsBlocked = true;

            grille[13, 9].IsBlocked = true;
            grille[14, 9].IsBlocked = true;

            grille[3, 6].IsBlocked = true;
            grille[11, 6].IsBlocked = true;

            for (int i = 4; i < 11; ++i)
                grille[i, 6].IsBlocked = true;

            this.grille = grille;
            allGrille.Add(grille);
            List<Vector2> objectPosition = new List<Vector2>();
            objectPosition.Add(new Vector2(137, 400));
            objectPosition.Add(new Vector2(454, 300));
            objectPosition.Add(new Vector2(774, 400));
            objectPosition.Add(new Vector2(45, 200));
            objectPosition.Add(new Vector2(871, 200));
            objectPosition.Add(new Vector2(460, 100));
            allAvailableSpawnPosition.Add(objectPosition);
        }

        private void initMap2()
        {
            Tile[,] grille = new Tile[columns, rows];
            InitializeAllTilesAndBlockSomeRandomly(grille);
            setFloorBlocked(grille);

            for (int x = 6; x < 9; ++x)
                grille[x, 15].IsBlocked = true;

            for(int i = 15; i > 11; --i)
            {
                grille[0, i].IsBlocked = true;
                grille[2, i].IsBlocked = true;
                grille[14, i].IsBlocked = true;
                grille[12, i].IsBlocked = true;
            }

            grille[1, 12].IsBlocked = true;
            grille[13, 12].IsBlocked = true;

            for (int i = 4; i < 11; ++i)
            {
                grille[i, 9].IsBlocked = true;
            }

            grille[0, 6].IsBlocked = true;
            grille[1, 6].IsBlocked = true;
            grille[2, 6].IsBlocked = true;
            grille[14, 6].IsBlocked = true;
            grille[13, 6].IsBlocked = true;
            grille[12, 6].IsBlocked = true;

            grille[6, 5].IsBlocked = true;
            grille[7, 5].IsBlocked = true;
            grille[8, 5].IsBlocked = true;

            this.grille = grille;
            allGrille.Add(grille);

            List<Vector2> objectPosition = new List<Vector2>();
            objectPosition.Add(new Vector2(70, 420));
            objectPosition.Add(new Vector2(453, 220));
            objectPosition.Add(new Vector2(845, 420));
            objectPosition.Add(new Vector2(70, 100));
            objectPosition.Add(new Vector2(845, 100));
            objectPosition.Add(new Vector2(460, 110));
            allAvailableSpawnPosition.Add(objectPosition);
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
