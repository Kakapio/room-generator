using System;
using Microsoft.Xna.Framework;

namespace RoomGenerator
{
    public class Eater
    {
        public Vector2 Direction { get; private set; }
        public Vector2 Position { get; private set; }
        public event Action<Vector2> AddEater;

        private Random random;
        private const int SpawnEaterChance = 400; //Higher value = less likely to spawn another.

        public Eater(Vector2 position)
        {
            GenerateDirection();
            Position = position;
            random = new Random();
        }

        /// <summary>
        /// Move the Eater in its Direction by one step, remove whatever wall is in the way.
        /// </summary>
        public void TryMove()
        {
            var newPos = Position + Direction;

            //Invalid direction, change direction and return.
            if (newPos.X < 0 || newPos.Y < 0 
                             || newPos.X > RoomGenerator.MapSize - 1 || newPos.Y > RoomGenerator.MapSize - 1)
            {
                GenerateDirection();
                return;
            }

            Position = newPos;
            
            if (RoomGenerator.TileMap[(int) Position.X, (int) Position.Y] != RoomGenerator.TileType.Floor)
            {
                RoomGenerator.TileMap[(int) Position.X, (int) Position.Y] = RoomGenerator.TileType.Floor;
                RoomGenerator.CurrentFloors--;
            }

            int genNewEater = random.Next(SpawnEaterChance);
            if (genNewEater == 0) // 1/SpawnEaterChance of spawning new eater.
            {
                AddEater?.Invoke(Position); 
            }
            
            GenerateDirection();
        }

        /// <summary>
        /// Generate a new direction for the eater.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void GenerateDirection()
        {
            Random rand = new Random();
            int dir = rand.Next(4);

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