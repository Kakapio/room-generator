using System;
using Microsoft.Xna.Framework;

namespace RoomGenerator
{
    public class Eater
    {
        public Vector2 Direction { get; private set; }
        public Vector2 Position { get; private set; }

        public Eater(Vector2 direction, Vector2 position)
        {
            Direction = direction;
            Position = position;
        }

        /// <summary>
        /// Move the Eater in its Direction by one step, remove whatever wall is in the way.
        /// </summary>
        public void Move()
        {
            var newPos = Position + Direction;
            //Invalid direction, change direction and return.
            if (newPos.X < 0 || newPos.Y < 0 || newPos.X > Game1.MapSize - 1 || newPos.Y > Game1.MapSize - 1
            || Game1.CurrentFloors <= 0)
            {
                GenerateDirection();
                return;
            }

            Position = newPos;
            Game1.TileMap[(int) Position.X, (int) Position.Y] = Game1.TileType.Floor;
            Game1.CurrentFloors--;
            
            GenerateDirection();
        }

        /// <summary>
        /// Generate a new direction for the eater.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void GenerateDirection()
        {
            Random random = new Random();
            int dir = random.Next(4);

            switch (dir)
            {
                //Left
                case 0:
                    Direction = new Vector2(-1, 0);
                    break;
                //Up
                case 1:
                    Direction = new Vector2(0, 1);
                    break;
                //Right
                case 2:
                    Direction = new Vector2(1, 0);
                    break;
                //Down
                case 3:
                    Direction = new Vector2(0, -1);
                    break;
                default:
                    throw new ArgumentException("Invalid value generated for new direction.");
            }
        }
    }
}