using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RoomGenerator
{
    public enum TileType
    {
        Floor,
        Wall
    }
    
    public class RoomGenerator : Game, IRoomGenerator
    {
        //Variables that are accessed by the eater class.
        public TileType[,] TileMap { get; }
        public int CurrentFloors { get; set; } = MaxFloors;
        public const int MapSize = 100; //The square dimensions of the tile map in terms of tiles.
        
        private const int MaxFloors = 1200;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private List<Eater> eaters;
        private Texture2D floor;
        private Texture2D wall;
        private const int TileSize = 8; //The square dimensions of each tile sprite.
        
        //Properties for cleaner code
        private bool ExitRequested => GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                                      || Keyboard.GetState().IsKeyDown(Keys.Escape);
        private bool RestartRequested => Keyboard.GetState().IsKeyDown(Keys.R);
        private bool EnoughWallsRemoved => CurrentFloors <= 0;
        
        public RoomGenerator()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            TileMap = new TileType[MapSize, MapSize];
            eaters = new List<Eater>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            //Resolution of window.
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.ApplyChanges();
            
            Reset();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            floor = Content.Load<Texture2D>("Ground");
            wall = Content.Load<Texture2D>("Wall");
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (ExitRequested)
                Exit();

            if (RestartRequested)
                Reset();
            
            Console.WriteLine($"Number floors created: {MaxFloors - CurrentFloors}, " +
                              $"Number Eaters: {eaters.Count}");

            PrintTileCount();

            if (MoveEaters()) 
                return;

            AddChildEaters();
        }

        private bool MoveEaters()
        {
            foreach (var eater in eaters)
            {
                //End condition check placed here to ensure too many walls are not removed.
                if (EnoughWallsRemoved)
                    return true;

                eater.TryMove();
            }

            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            GraphicsDevice.Clear(Color.SkyBlue);
            
            //Rendering to a RenderTarget and then later passing it to the back buffer.
            RenderTarget2D target = new RenderTarget2D(GraphicsDevice, MapSize * TileSize, MapSize * TileSize);
            GraphicsDevice.SetRenderTarget(target);
            
            spriteBatch.Begin();

            //Draw each tile in the proper position.
            for (int i = 0; i < TileMap.GetLength(0); i++)
            {
                for (int j = 0; j < TileMap.GetLength(1); j++)
                {
                    switch (TileMap[i, j])
                    {
                        case TileType.Floor:
                            spriteBatch.Draw(floor, 
                                new Rectangle(i * TileSize, j * TileSize, TileSize, TileSize), 
                                Color.White);
                            break;
                        case TileType.Wall:
                            spriteBatch.Draw(wall, 
                                new Rectangle(i * TileSize, j * TileSize, TileSize, TileSize), 
                                Color.White);
                            break;
                        default:
                            throw new InvalidEnumArgumentException("Given tile type is not supported.");
                    }
                }
            }
            
            spriteBatch.End();
            
            //Render target to back buffer.
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);
            spriteBatch.Draw(target, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
        }
        
        private void AddChildEaters()
        {
            for (int i = 0; i < eaters.Count; i++)
            {
                if (! eaters[i].HasChild)
                    return;

                eaters.Add(new Eater(eaters[i].Position, this));
                eaters[i].HasChild = false;
            }
        }

        public Boolean IsAvailableToMove(Vector2 position)
        {
            if (position.X < 0 || position.Y < 0 
                               || position.X > MapSize - 1 || position.Y > MapSize - 1)
            {
                return false;
            }

            return true;
        }
        
        private void PrintTileCount()
        {
            int trueCount = 0;

            for (int i = 0; i < TileMap.GetLength(0); i++)
            {
                for (int j = 0; j < TileMap.GetLength(1); j++)
                {
                    if (TileMap[i, j] == TileType.Floor)
                        trueCount++;
                }
            }

            Console.WriteLine($"True Count: {trueCount}");
        }

        private void Reset()
        {
            //Set all tiles back to wall.
            for (int i = 0; i < TileMap.GetLength(0); i++)
            {
                for (int j = 0; j < TileMap.GetLength(1); j++)
                {
                    TileMap[i, j] = TileType.Wall;
                }
            }

            CurrentFloors = MaxFloors;
            eaters.Clear();
            
            eaters.Add(new Eater(new Vector2(MapSize / 2, MapSize / 2), this));
        }
    }
}