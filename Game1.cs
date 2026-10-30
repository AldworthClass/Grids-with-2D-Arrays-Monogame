using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data;

namespace Grids_with_2D_Arrays_Monogame
{
    public enum TileState
    {
        Mowed, 
        Grass,
        Dandelion
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Random generator;

        KeyboardState keyboardState;
        Rectangle window;

        Texture2D dandelionTexture;

        Texture2D grassTexture;
        List<Rectangle> grassTiles;

        Texture2D mowerTexture;
        Rectangle mowerRect;
        SoundEffect mowerSound;
        SoundEffectInstance mowerSoundInstance;
        Vector2 mowerSpeed;

        int tileSize;

        TileState[,] tileItems;
        float dandeLionPercentage;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            generator = new Random();

             window = new Rectangle(0, 0, 600, 500);
            _graphics.PreferredBackBufferWidth = window.Width;
            _graphics.PreferredBackBufferHeight = window.Height;
            _graphics.ApplyChanges();

            dandeLionPercentage = 0.01f;    // 1% chance of grass tile having a dandelion
            tileSize = 5;                   // Tiles are 5 x 5 pixels

            // Creates map
            tileItems = new TileState[100, 120];
            for (int row = 0; row <  100; row++)
                for (int column = 0; column < 120; column++)
                {
                    // Determines whethere tile will be a dandelion
                    if (generator.NextDouble() < dandeLionPercentage)
                        tileItems[row, column] = TileState.Dandelion;
                    else
                        tileItems[row, column] = TileState.Grass;
                }

            mowerRect = new Rectangle(100, 100, 30, 30);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            
            grassTexture = Content.Load<Texture2D>("Images/long_grass");
            mowerTexture = Content.Load<Texture2D>("Images/mower");
            dandelionTexture = Content.Load<Texture2D>("Images/dandelion");

            mowerSound = Content.Load<SoundEffect>("Sounds/mower_sound");
            mowerSoundInstance = mowerSound.CreateInstance();
            mowerSoundInstance.IsLooped = true;		// Sound will play until we stop it

        }

        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Handles mower movement
            mowerSpeed = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.W))
                mowerSpeed.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S))
                mowerSpeed.Y += 1;
            if (keyboardState.IsKeyDown(Keys.A))
                mowerSpeed.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D))
                mowerSpeed.X += 1;

            // Plays sound when mower is moving
            if (mowerSpeed == Vector2.Zero)
                mowerSoundInstance.Stop();
            else
                mowerSoundInstance.Play();

            mowerRect.Offset(mowerSpeed);

            // Iterates through tiles to cut grass
            for (int row = 0; row < tileItems.GetLength(0); row++)
                for (int col = 0; col < tileItems.GetLength(1); col++)
                {
                    if (mowerRect.Contains(GetTileRect(row, col, tileSize)))
                        tileItems[row, col] = TileState.Mowed;
                }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGreen);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            // Iterates through tilemap to draw individual tiles
            for (int row = 0; row < tileItems.GetLength(0); row++)
                for (int col = 0; col < tileItems.GetLength(1); col++)
                {
                    if (tileItems[row, col] == TileState.Grass)
                        _spriteBatch.Draw(grassTexture, GetTileRect(row, col, tileSize), Color.White);
                    else if (tileItems[row, col] == TileState.Dandelion)
                    {
                        _spriteBatch.Draw(grassTexture, GetTileRect(row, col, tileSize), Color.White);
                        _spriteBatch.Draw(dandelionTexture, GetTileRect(row, col, tileSize), Color.White);
                    }

                }
            

            _spriteBatch.Draw(mowerTexture, mowerRect, Color.White);

            _spriteBatch.End();


            base.Draw(gameTime);
        }

        // When given a row, column and tile size, returns a Rectangle for that tile
        // This can be used for collision and drawing
        public Rectangle GetTileRect(int row, int col, int size)
        {
            return new Rectangle(col * size, row * size, size, size);
        }
    }
}
