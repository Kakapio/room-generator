using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RoomGenerator
{
    public class Game1 : Game
    {
        enum TileType
        {
            Floor,
            Wall
        }
        
        class Eater
        {
            private Vector2 Direction { get; private set; }
            private Vector2 Position { get; private set; }

            public Eater(Vector2 direction, Vector2 position)
            {
                Direction = direction;
                Position = position;
            }

            public void Move()
            {
                var newPos = Position + Direction;
                if (newPos.X < 0 || newPos.Y < 0 || newPos.X > mapSize - 1 || newPos.Y > mapSize - 1)
                {
                    return;
                }
                
                Position += Direction;
            }

            public void ChangeDirection()
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
        
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private TileType[,] tilemap;
        private List<Eater> eaters;
        private int mapSize = 100; //The square dimensions of the tile map in terms of tiles.
        private Texture2D floor;
        private Texture2D wall;
        private int tileSize = 8; //The square dimensions of each tile sprite.
        
        //Parameters to modify map generation
        private int maxFloorAmount = 120;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            tilemap = new TileType[mapSize, mapSize];
            eaters = new List<Eater>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            //Resolution of window.
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.ApplyChanges();
            
            //Pre-populate tilemap with base tile.
            for (int i = 0; i < tilemap.GetLength(0); i++)
            {
                for (int j = 0; j < tilemap.GetLength(1); j++)
                {
                    tilemap[i, j] = TileType.Wall;
                }
            }
            
            eaters.Add(new Eater());
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
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            GraphicsDevice.Clear(Color.SkyBlue);
            
            //Rendering to a RenderTarget and then later passing it to the back buffer.
            RenderTarget2D target = new RenderTarget2D(GraphicsDevice, mapSize * tileSize, mapSize * tileSize);
            GraphicsDevice.SetRenderTarget(target);
            
            spriteBatch.Begin();

            //Draw each tile in the proper position.
            for (int i = 0; i < tilemap.GetLength(0); i++)
            {
                for (int j = 0; j < tilemap.GetLength(1); j++)
                {
                    switch (tilemap[i, j])
                    {
                        case TileType.Floor:
                            spriteBatch.Draw(floor, 
                                new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), 
                                Color.White);
                            break;
                        case TileType.Wall:
                            spriteBatch.Draw(wall, 
                                new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), 
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
    }
}
