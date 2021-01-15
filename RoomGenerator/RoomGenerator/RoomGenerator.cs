using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RoomGenerator
{
    public class RoomGenerator : Game
    {
        public enum TileType
        {
            Floor,
            Wall
        }

        //Variables that are accessed by the eater class.
        public static TileType[,] TileMap { get; set; }
        public static int CurrentFloors = MaxFloors;
        public const int MapSize = 100; //The square dimensions of the tile map in terms of tiles.
        
        private const int MaxFloors = 1200;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private List<Eater> eaters;
        private List<Eater> eatersAccumulator; //Used to ensure an enumeration operation error does not occur.
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
            eatersAccumulator = new List<Eater>();
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
            
            if (EnoughWallsRemoved)
                return;

            foreach (var eater in eaters)
            {
                eater.TryMove();
            }

            foreach (var eater in eatersAccumulator)
            {
                eaters.Add(eater);
            }
            eatersAccumulator.Clear();
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
            
            eaters.Add(new Eater(new Vector2(MapSize / 2, MapSize / 2)));
            eaters[0].AddEater += AddEater;
        }

        /// <summary>
        /// Triggered by Eaters to add more eaters to the generator. Allows for generation of separate corridors.
        /// </summary>
        private void AddEater(Vector2 startPosition)
        {
            Eater eater = new Eater(startPosition);
            eater.GenerateDirection();
            eater.AddEater += AddEater; //Subscribe new eater to this method.
            
            eatersAccumulator.Add(eater);
        }
    }
}
