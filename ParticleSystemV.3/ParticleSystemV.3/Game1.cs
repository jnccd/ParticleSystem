using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BloomPostprocess;

namespace ParticleSystemV._3
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BloomComponent bloom;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int)GameValues.ScreenSize.X;
            graphics.PreferredBackBufferHeight = (int)GameValues.ScreenSize.Y;
            bloom = new BloomComponent(this);
            Components.Add(bloom);
            bloom.Settings = new BloomSettings(null, 0.001f, 2, 2, 1, 1.5f, 1);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Assets.Load(GraphicsDevice, Content);
            ParticleManager.Create();
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GameStorage.game.IsActive)
                ControlHandler.Update();

            EingabenAnzeige.Update();

            if (GameValues.MultiThreading)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ParticleManager.ThreadedUpdate0));
                ThreadPool.QueueUserWorkItem(new WaitCallback(ParticleManager.ThreadedUpdate1));
                ThreadPool.QueueUserWorkItem(new WaitCallback(ParticleManager.ThreadedUpdate2));
                ThreadPool.QueueUserWorkItem(new WaitCallback(ParticleManager.ThreadedUpdate3));
            }
            else
            {
                ParticleManager.Update();
            }

            FPSCounter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (GameValues.Bloom)
            {
                bloom.Visible = true;
            }
            else
            {
                bloom.Visible = false;
            }
            bloom.BeginDraw();
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            switch (GameValues.DrawMethod)
            {
                case 0:
                    ParticleManager.DrawAsPoints(spriteBatch);
                    break;

                case 1:
                    ParticleManager.DrawAsArrows(spriteBatch);
                    break;

                case 2:
                    ParticleManager.DrawAsWater(spriteBatch);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
            spriteBatch.Begin();

            ParticleManager.DrawText(spriteBatch);
            FPSCounter.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
