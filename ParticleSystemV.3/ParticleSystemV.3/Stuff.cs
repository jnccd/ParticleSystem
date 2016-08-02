﻿using System;
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

namespace ParticleSystemV._3
{
    public static class GameValues
    {
        public static Vector2 ScreenSize = new Vector2(1360, 768);
        public static float GravForce = 3000;
        public static float Friction = 1.005f;
        public static bool FrictionEnabled = true;
        public static Random RDM = new Random();
        public static int ClickCooldown = 0;
        public static int GravityMode = 0;
        public static bool MultiThreading = true;
        public static int DrawMethod = 0;
        public static bool Bloom = true;
        public static int WaterSize = 15;
        public static bool particleCollision = false;
    }

    public static class Assets
    {
        public static Texture2D Default;
        public static Texture2D Arrow;
        public static Texture2D BigCircle;
        public static Texture2D SmallerCircle;
        public static SpriteFont Font;

        public static void Load(GraphicsDevice graphicsDevice, ContentManager Content)
        {
            Default = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] Col = new Color[1];
            Col[0] = new Color(255, 255, 255, 255);
            Default.SetData<Color>(Col);

            Arrow = Content.Load<Texture2D>("Arrow");
            BigCircle = Content.Load<Texture2D>("BigCircle");
            SmallerCircle = Content.Load<Texture2D>("SmallerCircle");

            Font = Content.Load<SpriteFont>("SpriteFont");
        }
    }

    public static class ControlHandler
    {
        public static MouseState CurMS;
        public static MouseState LastMS;
        public static KeyboardState CurKS;
        public static KeyboardState LastKS;

        public static float Angle;
        public static float Strength;

        static int PressedFTimer = 0;

        public static void Update()
        {
            if (CurMS != null)
            {
                LastMS = CurMS;
            }

            if (CurKS != null)
            {
                LastKS = CurKS;
            }

            CurMS = Mouse.GetState();
            CurKS = Keyboard.GetState();

            if (GameValues.ClickCooldown > 0)
            {
                GameValues.ClickCooldown--;
            }
        }

        public static Vector2 GetMouseVector() { return new Vector2(CurMS.X, CurMS.Y); }

        public static void HandleControls(List<Particle> ListOfParticles, List<Marker> ListOfMarkers)
        {
            if (ControlHandler.CurKS.IsKeyDown(Keys.V))
            {
                EingabenAnzeige.SetNewText("Gave all Particles a Velocity of: " + Vector2.Zero.ToString());
                lock (ListOfParticles)
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].Vel = Vector2.Zero;
                    }
                }

                lock (ListOfMarkers)
                {
                    for (int i = 0; i < ListOfMarkers.Count; i++)
                    {
                        ListOfMarkers[i].Vel = Vector2.Zero;
                    }
                }
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.J))
            {
                lock (ListOfParticles)
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].Pos += new Vector2(GameValues.RDM.Next(-3, 4), GameValues.RDM.Next(-3, 4));
                    }
                }
            }

            if (ControlHandler.CurMS.LeftButton == ButtonState.Pressed && GameValues.ClickCooldown == 0)
            {
                lock (ListOfParticles)
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].GetPulledBy(new Vector2(ControlHandler.CurMS.X, ControlHandler.CurMS.Y), true, 2f);
                    }
                }

                GameValues.ClickCooldown = 7;
            }

            if (ControlHandler.CurMS.RightButton == ButtonState.Pressed && ControlHandler.LastMS.RightButton == ButtonState.Released)
            {
                lock (ListOfParticles)
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].GetPulledBy(new Vector2(ControlHandler.CurMS.X, ControlHandler.CurMS.Y), false, 5f);
                    }
                }
            }

            if (ControlHandler.CurMS.MiddleButton == ButtonState.Pressed && GameValues.ClickCooldown == 0 ||
                CurKS.IsKeyDown(Keys.Z) && GameValues.ClickCooldown == 0)
            {
                lock (ListOfParticles)
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].OrbitAround(new Vector2(ControlHandler.CurMS.X, ControlHandler.CurMS.Y), false);
                    }
                }

                GameValues.ClickCooldown = 7;
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.E))
            {
                lock (ListOfParticles)
                {
                    ListOfParticles.Add(new Particle(ControlHandler.CurMS.X, ControlHandler.CurMS.Y, 10, 0));
                }
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.D))
            {
                EingabenAnzeige.SetNewText("Deleted all Particles");
                lock (ListOfParticles)
                {
                    ListOfParticles.Clear();
                }

                lock (ListOfMarkers)
                {
                    ListOfMarkers.Clear();
                }
            }

            if (CurKS.IsKeyDown(Keys.O))
            {
                if (ParticleManager.ParticleVisibility < 1)
                {
                    ParticleManager.ParticleVisibility += 0.004f;
                    EingabenAnzeige.SetNewText("Increaseing Particle visibility to " + ParticleManager.ParticleVisibility.ToString());
                }
            }

            if (CurKS.IsKeyDown(Keys.L))
            {
                if (ParticleManager.ParticleVisibility > 0)
                {
                    ParticleManager.ParticleVisibility -= 0.004f;
                    EingabenAnzeige.SetNewText("Lowering Particle visibility " + ParticleManager.ParticleVisibility.ToString());
                }
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.D0))
            {
                EingabenAnzeige.SetNewText("Gave all Particles Position: " + (GameValues.ScreenSize / 2).ToString());
                lock (ListOfParticles)
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].Pos = GameValues.ScreenSize / 2;
                    }
                }

                lock (ListOfMarkers)
                {
                    for (int i = 0; i < ListOfMarkers.Count; i++)
                    {
                        ListOfMarkers[i].Vel = Vector2.Zero;
                    }
                }
            }

            if (CurKS.IsKeyDown(Keys.PageUp) && ControlHandler.LastKS.IsKeyUp(Keys.PageUp))
            {
                EingabenAnzeige.SetNewText("Spawned Proton");
                lock (ListOfMarkers)
                {
                    ListOfMarkers.Add(new Marker(new Vector2(ControlHandler.CurMS.X, ControlHandler.CurMS.Y), Vector2.Zero, true));
                }
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.PageDown) && ControlHandler.LastKS.IsKeyUp(Keys.PageDown))
            {
                EingabenAnzeige.SetNewText("Spawned Electron");
                lock (ListOfMarkers)
                {
                    ListOfMarkers.Add(new Marker(new Vector2(ControlHandler.CurMS.X, ControlHandler.CurMS.Y), Vector2.Zero, false));
                }
            }

            if (CurKS.IsKeyDown(Keys.R) && LastKS.IsKeyUp(Keys.R))
            {
                EingabenAnzeige.SetNewText("Gave Particles Random Velocity");
                lock (ListOfParticles)
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].Vel = new Vector2(GameValues.RDM.Next(-5, 5), GameValues.RDM.Next(-5, 5));
                    }
                }
            }

            if (CurKS.IsKeyDown(Keys.S))
            {
                EingabenAnzeige.SetNewText("Particles Reset");

                lock (ListOfParticles)
                {
                    ListOfParticles.Clear();

                    for (int ix = 0; ix < GameValues.ScreenSize.X; ix += 3)
                    {
                        for (int iy = 0; iy < GameValues.ScreenSize.Y; iy += 3)
                        {
                            ListOfParticles.Add(new Particle(ix, iy, 0, 0, 1f, -0.01f));
                        }
                    }
                }
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.Q) && ControlHandler.LastKS.IsKeyUp(Keys.Q))
            {
                EingabenAnzeige.SetNewText("Spawned Rectangle");

                lock (ListOfParticles)
                {
                    for (float ix = -100; ix < 100; ix += 0.1f)
                    {
                        ListOfParticles.Add(new Particle(new Vector2(CurMS.X + ix, CurMS.Y - 100), Vector2.Zero));
                        ListOfParticles.Add(new Particle(new Vector2(CurMS.X + ix, CurMS.Y + 100), Vector2.Zero));
                    }

                    for (float iy = -100; iy < 100; iy += 0.1f)
                    {
                        ListOfParticles.Add(new Particle(new Vector2(CurMS.X - 100, CurMS.Y + iy), Vector2.Zero));
                        ListOfParticles.Add(new Particle(new Vector2(CurMS.X + 100, CurMS.Y + iy), Vector2.Zero));
                    }
                }
            }

            if (CurKS.IsKeyDown(Keys.H) && LastKS.IsKeyUp(Keys.H))
            {
                EingabenAnzeige.SetNewText("Spawned Circle");

                lock (ListOfParticles)
                {
                    for (double Angle = 0; Angle < Math.PI*2; Angle += 0.001)
                    {
                        ListOfParticles.Add(new Particle(new Vector2((float)Math.Sin(Angle) * 100, (float)Math.Cos(Angle) * 100) + GetMouseVector(), Vector2.Zero));
                    }
                }
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.Space) && ControlHandler.LastKS.IsKeyUp(Keys.Space))
            {
                lock (ListOfParticles)
                {
                    for (float i = 0; i < GameValues.ScreenSize.X; i += 0.1f)
                    {
                        ListOfParticles.Add(new Particle(new Vector2(i, CurMS.Y + i / 10), new Vector2(0.25f, 0)));
                        ListOfParticles.Add(new Particle(new Vector2(i, CurMS.Y + i / 10), new Vector2(0.25f, 0)));
                    }
                }
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.P))
            {
                lock (ListOfParticles)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ListOfParticles.Add(new Particle(ControlHandler.CurMS.X + GameValues.RDM.Next(-10, 10), ControlHandler.CurMS.Y + GameValues.RDM.Next(-10, 10), 0, 0, 0.5f, -0.01f));
                    }
                }
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.F))
            {
                PressedFTimer++;
            }

            if (ControlHandler.CurKS.IsKeyUp(Keys.F) && ControlHandler.LastKS.IsKeyDown(Keys.F) && PressedFTimer < 300)
            {
                GameValues.FrictionEnabled = !GameValues.FrictionEnabled;
                EingabenAnzeige.SetNewText("Friction: " + GameValues.FrictionEnabled.ToString());
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.F) && ControlHandler.CurKS.IsKeyDown(Keys.OemPlus))
            {
                GameValues.Friction += 0.0001f;
                EingabenAnzeige.SetNewText("Friction increased to " + GameValues.Friction.ToString());
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.F) && ControlHandler.CurKS.IsKeyDown(Keys.OemMinus))
            {
                GameValues.Friction -= 0.0001f;
                EingabenAnzeige.SetNewText("Friction lowered to " + GameValues.Friction.ToString());
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.G) && ControlHandler.LastKS.IsKeyUp(Keys.G))
            {
                switch (GameValues.GravityMode)
                {
                    case 0:
                        GameValues.GravityMode = 1;
                        break;

                    case 1:
                        GameValues.GravityMode = 2;
                        break;

                    case 2:
                        GameValues.GravityMode = 0;
                        break;
                }
                EingabenAnzeige.SetNewText("GravityMode: " + GameValues.GravityMode.ToString());
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.B) && ControlHandler.LastKS.IsKeyUp(Keys.B))
            {
                GameValues.Bloom = !GameValues.Bloom;
                EingabenAnzeige.SetNewText("Bloom: " + GameValues.Bloom.ToString());
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.C) && ControlHandler.LastKS.IsKeyUp(Keys.C))
            {
                GameValues.particleCollision = !GameValues.particleCollision;
                EingabenAnzeige.SetNewText("ParticleCollision: " + GameValues.particleCollision.ToString());
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.Y) && ControlHandler.LastKS.IsKeyUp(Keys.Y))
            {
                switch (GameValues.DrawMethod)
                {
                    case 0:
                        GameValues.DrawMethod = 1;
                        break;

                    case 1:
                        GameValues.DrawMethod = 2;
                        break;

                    case 2:
                        GameValues.DrawMethod = 0;
                        break;
                }
                EingabenAnzeige.SetNewText("DrawMethod: " + GameValues.DrawMethod.ToString());
            }

            if (ControlHandler.CurKS.IsKeyDown(Keys.M) && ControlHandler.LastKS.IsKeyUp(Keys.M))
            {
                GameValues.MultiThreading = !GameValues.MultiThreading;
                EingabenAnzeige.SetNewText("Multithreading set to: " + GameValues.MultiThreading.ToString());
            }
        }
    }
}
