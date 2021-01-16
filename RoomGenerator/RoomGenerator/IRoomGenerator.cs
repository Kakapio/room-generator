using System;
using Microsoft.Xna.Framework;

namespace RoomGenerator
{
    public interface IRoomGenerator
    {
        public Boolean IsAvailableToMove(Vector2 position);
        
        public TileType[,] TileMap { get; }
        public int CurrentFloors { get; set; }
    }
}