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

namespace ParticleSystemV._3
{
    public static class ParticleManager
    {
        public static List<Particle> ListOfParticles = new List<Particle>();
        public static List<Marker> ListOfMarkers = new List<Marker>();
        public static Vector2 Camera = Vector2.Zero;
        static double Angle;
        static float PullLength;
        static float ForceMagnitude;
        static float ForceAngle;
        const float WaterRepelSize = 3f;
        const float WaterRepelStrength = 0.02f;

        public static float ParticleVisibility = 0.25f;

        public static void Create()
        {
            for (int ix = 0; ix < GameValues.ScreenSize.X; ix += 3)
            {
                for (int iy = 0; iy < GameValues.ScreenSize.Y; iy += 3)
                {
                    ListOfParticles.Add(new Particle(ix, iy, 0, 0));
                }
            }
        }

        public static void Update()
        {
            ControlHandler.HandleControls(ListOfParticles, ListOfMarkers);

            for (int i = 0; i < ListOfParticles.Count; i++)
            {
                ListOfParticles[i].Update();
            }

            if (GameValues.particleCollision)
            {
                lock (ListOfParticles)
                {
                    for (int i1 = 0; i1 < ListOfParticles.Count; i1++)
                    {
                        for (int i2 = 0; i2 < ListOfParticles.Count; i2++)
                        {
                            if (ListOfParticles[i1].Pos.X - GameValues.WaterSize < ListOfParticles[i2].Pos.X && ListOfParticles[i1].Pos.X + GameValues.WaterSize > ListOfParticles[i2].Pos.X &&
                                ListOfParticles[i1].Pos.Y - GameValues.WaterSize < ListOfParticles[i2].Pos.Y && ListOfParticles[i1].Pos.Y + GameValues.WaterSize > ListOfParticles[i2].Pos.Y)
                            {
                                double ZwischenSpeicher1 = Convert.ToDouble(ListOfParticles[i1].Pos.X - ListOfParticles[i2].Pos.X);
                                double ZwischenSpeicher2 = Convert.ToDouble(ListOfParticles[i1].Pos.Y - ListOfParticles[i2].Pos.Y);

                                if (ZwischenSpeicher1 * ZwischenSpeicher1 + ZwischenSpeicher2 * ZwischenSpeicher2 < GameValues.WaterSize * GameValues.WaterSize / WaterRepelSize)
                                {
                                    ListOfParticles[i1].Vel.X += Convert.ToSingle(ZwischenSpeicher1) * WaterRepelStrength;
                                    ListOfParticles[i1].Vel.Y += Convert.ToSingle(ZwischenSpeicher2) * WaterRepelStrength;
                                }
                            }

                            //if (Vector2.Distance(ListOfParticles[i1].Pos, ListOfParticles[i2].Pos) < GameValues.WaterSize)
                            //{
                            //    ListOfParticles[i2].GetPulledBy(ListOfParticles[i1].Pos, true, 0.5f);
                            //}
                        }
                    }
                }
            }

            for (int i = 0; i < ListOfMarkers.Count; i++)
            {
                ListOfMarkers[i].Update();
                ListOfMarkers[i].MarkerUpdate(ListOfMarkers, ListOfParticles);
            }
        }

        public static void ThreadedUpdate0(object o)
        {
            ControlHandler.HandleControls(ListOfParticles, ListOfMarkers);

            lock (ListOfParticles)
            {
                for (int i = 0; i < ListOfParticles.Count / 4; i++)
                {
                    ListOfParticles[i].Update();
                }
            }

            if (GameValues.particleCollision)
            {
                lock (ListOfParticles)
                {
                    for (int i1 = 0; i1 < ListOfParticles.Count / 4; i1++)
                    {
                        for (int i2 = 0; i2 < ListOfParticles.Count; i2++)
                        {
                            if (ListOfParticles[i1].Pos.X - GameValues.WaterSize < ListOfParticles[i2].Pos.X && ListOfParticles[i1].Pos.X + GameValues.WaterSize > ListOfParticles[i2].Pos.X &&
                                ListOfParticles[i1].Pos.Y - GameValues.WaterSize < ListOfParticles[i2].Pos.Y && ListOfParticles[i1].Pos.Y + GameValues.WaterSize > ListOfParticles[i2].Pos.Y)
                            {
                                double ZwischenSpeicher1 = Convert.ToDouble(ListOfParticles[i1].Pos.X - ListOfParticles[i2].Pos.X);
                                double ZwischenSpeicher2 = Convert.ToDouble(ListOfParticles[i1].Pos.Y - ListOfParticles[i2].Pos.Y);

                                if (ZwischenSpeicher1 * ZwischenSpeicher1 + ZwischenSpeicher2 * ZwischenSpeicher2 < GameValues.WaterSize * GameValues.WaterSize / WaterRepelSize)
                                {
                                    ListOfParticles[i1].Vel.X += Convert.ToSingle(ZwischenSpeicher1) * WaterRepelStrength;
                                    ListOfParticles[i1].Vel.Y += Convert.ToSingle(ZwischenSpeicher2) * WaterRepelStrength;
                                }
                            }

                            //if (Vector2.Distance(ListOfParticles[i1].Pos, ListOfParticles[i2].Pos) < GameValues.WaterSize)
                            //{
                            //    ListOfParticles[i2].GetPulledBy(ListOfParticles[i1].Pos, true, 0.5f);
                            //}
                        }
                    }
                }
            }

            lock (ListOfMarkers)
            {
                for (int i = 0; i < ListOfMarkers.Count / 4; i++)
                {
                    ListOfMarkers[i].Update();
                    ListOfMarkers[i].MarkerUpdate(ListOfMarkers, ListOfParticles);

                }
            }
        }
        public static void ThreadedUpdate1(object o)
        {
            lock (ListOfParticles)
            {
                for (int i = ListOfParticles.Count / 4; i < ListOfParticles.Count / 2; i++)
                {
                    ListOfParticles[i].Update();
                }
            }

            if (GameValues.particleCollision)
            {
                lock (ListOfParticles)
                {
                    for (int i1 = ListOfParticles.Count / 4; i1 < ListOfParticles.Count / 2; i1++)
                    {
                        for (int i2 = 0; i2 < ListOfParticles.Count; i2++)
                        {
                            if (ListOfParticles[i1].Pos.X - GameValues.WaterSize < ListOfParticles[i2].Pos.X && ListOfParticles[i1].Pos.X + GameValues.WaterSize > ListOfParticles[i2].Pos.X &&
                                ListOfParticles[i1].Pos.Y - GameValues.WaterSize < ListOfParticles[i2].Pos.Y && ListOfParticles[i1].Pos.Y + GameValues.WaterSize > ListOfParticles[i2].Pos.Y)
                            {
                                double ZwischenSpeicher1 = Convert.ToDouble(ListOfParticles[i1].Pos.X - ListOfParticles[i2].Pos.X);
                                double ZwischenSpeicher2 = Convert.ToDouble(ListOfParticles[i1].Pos.Y - ListOfParticles[i2].Pos.Y);

                                if (ZwischenSpeicher1 * ZwischenSpeicher1 + ZwischenSpeicher2 * ZwischenSpeicher2 < GameValues.WaterSize * GameValues.WaterSize / WaterRepelSize)
                                {
                                    ListOfParticles[i1].Vel.X += Convert.ToSingle(ZwischenSpeicher1) * WaterRepelStrength;
                                    ListOfParticles[i1].Vel.Y += Convert.ToSingle(ZwischenSpeicher2) * WaterRepelStrength;
                                }
                            }

                            //if (Vector2.Distance(ListOfParticles[i1].Pos, ListOfParticles[i2].Pos) < GameValues.WaterSize)
                            //{
                            //    ListOfParticles[i2].GetPulledBy(ListOfParticles[i1].Pos, true, 0.5f);
                            //}
                        }
                    }
                }
            }

            lock (ListOfMarkers)
            {
                for (int i = ListOfMarkers.Count / 4; i < ListOfMarkers.Count / 2; i++)
                {
                    ListOfMarkers[i].Update();
                    ListOfMarkers[i].MarkerUpdate(ListOfMarkers, ListOfParticles);
                }
            }
        }
        public static void ThreadedUpdate2(object o)
        {
            lock (ListOfParticles)
            {
                for (int i = ListOfParticles.Count / 2; i < (ListOfParticles.Count / 4) * 3; i++)
                {
                    ListOfParticles[i].Update();
                }
            }

            if (GameValues.particleCollision)
            {
                lock (ListOfParticles)
                {
                    for (int i1 = ListOfParticles.Count / 2; i1 < (ListOfParticles.Count / 4) * 3; i1++)
                    {
                        for (int i2 = 0; i2 < ListOfParticles.Count; i2++)
                        {
                            if (ListOfParticles[i1].Pos.X - GameValues.WaterSize < ListOfParticles[i2].Pos.X && ListOfParticles[i1].Pos.X + GameValues.WaterSize > ListOfParticles[i2].Pos.X &&
                                ListOfParticles[i1].Pos.Y - GameValues.WaterSize < ListOfParticles[i2].Pos.Y && ListOfParticles[i1].Pos.Y + GameValues.WaterSize > ListOfParticles[i2].Pos.Y)
                            {
                                double ZwischenSpeicher1 = Convert.ToDouble(ListOfParticles[i1].Pos.X - ListOfParticles[i2].Pos.X);
                                double ZwischenSpeicher2 = Convert.ToDouble(ListOfParticles[i1].Pos.Y - ListOfParticles[i2].Pos.Y);

                                if (ZwischenSpeicher1 * ZwischenSpeicher1 + ZwischenSpeicher2 * ZwischenSpeicher2 < GameValues.WaterSize * GameValues.WaterSize / WaterRepelSize)
                                {
                                    ListOfParticles[i1].Vel.X += Convert.ToSingle(ZwischenSpeicher1) * WaterRepelStrength;
                                    ListOfParticles[i1].Vel.Y += Convert.ToSingle(ZwischenSpeicher2) * WaterRepelStrength;
                                }
                            }

                            //if (Vector2.Distance(ListOfParticles[i1].Pos, ListOfParticles[i2].Pos) < GameValues.WaterSize)
                            //{
                            //    ListOfParticles[i2].GetPulledBy(ListOfParticles[i1].Pos, true, 0.5f);
                            //}
                        }
                    }
                }
            }

            lock (ListOfMarkers)
            {
                for (int i = ListOfMarkers.Count / 2; i < (ListOfMarkers.Count / 4) * 3; i++)
                {
                    ListOfMarkers[i].Update();
                    ListOfMarkers[i].MarkerUpdate(ListOfMarkers, ListOfParticles);
                }
            }
        }
        public static void ThreadedUpdate3(object o)
        {
            lock (ListOfParticles)
            {
                for (int i = (ListOfParticles.Count / 4) * 3; i < ListOfParticles.Count; i++)
                {
                    ListOfParticles[i].Update();
                }
            }

            if (GameValues.particleCollision)
            {
                lock (ListOfParticles)
                {
                    for (int i1 = (ListOfParticles.Count / 4) * 3; i1 < ListOfParticles.Count; i1++)
                    {
                        for (int i2 = 0; i2 < ListOfParticles.Count; i2++)
                        {
                            if (ListOfParticles[i1].Pos.X - GameValues.WaterSize < ListOfParticles[i2].Pos.X && ListOfParticles[i1].Pos.X + GameValues.WaterSize > ListOfParticles[i2].Pos.X &&
                                ListOfParticles[i1].Pos.Y - GameValues.WaterSize < ListOfParticles[i2].Pos.Y && ListOfParticles[i1].Pos.Y + GameValues.WaterSize > ListOfParticles[i2].Pos.Y)
                            {
                                double ZwischenSpeicher1 = Convert.ToDouble(ListOfParticles[i1].Pos.X - ListOfParticles[i2].Pos.X);
                                double ZwischenSpeicher2 = Convert.ToDouble(ListOfParticles[i1].Pos.Y - ListOfParticles[i2].Pos.Y);

                                if (ZwischenSpeicher1 * ZwischenSpeicher1 + ZwischenSpeicher2 * ZwischenSpeicher2 < GameValues.WaterSize * GameValues.WaterSize / WaterRepelSize)
                                {
                                    ListOfParticles[i1].Vel.X += Convert.ToSingle(ZwischenSpeicher1) * WaterRepelStrength;
                                    ListOfParticles[i1].Vel.Y += Convert.ToSingle(ZwischenSpeicher2) * WaterRepelStrength;
                                }
                            }

                            //if (Vector2.Distance(ListOfParticles[i1].Pos, ListOfParticles[i2].Pos) < GameValues.WaterSize)
                            //{
                            //    ListOfParticles[i2].GetPulledBy(ListOfParticles[i1].Pos, true, 0.5f);
                            //}
                        }
                    }
                }
            }

            lock (ListOfMarkers)
            {
                for (int i = (ListOfMarkers.Count / 4) * 3; i < ListOfMarkers.Count; i++)
                {
                    ListOfMarkers[i].Update();
                    ListOfMarkers[i].MarkerUpdate(ListOfMarkers, ListOfParticles);
                }
            }
        }

        public static void DrawAsPoints(SpriteBatch spriteBatch)
        {
            lock (ListOfParticles)
            {
                for (int i = 0; i < ListOfParticles.Count; i++)
                {
                    ListOfParticles[i].Draw(spriteBatch);
                }
            }

            for (int i = 0; i < ListOfMarkers.Count; i++)
            {
                ListOfMarkers[i].Draw(spriteBatch);
            }
        }
        public static void DrawAsArrows(SpriteBatch spriteBatch)
        {
            lock (ListOfParticles)
            {
                for (int i = 0; i < ListOfParticles.Count; i++)
                {
                    ListOfParticles[i].DrawAsArrow(spriteBatch);
                }
            }

            for (int i = 0; i < ListOfMarkers.Count; i++)
            {
                ListOfMarkers[i].Draw(spriteBatch);
            }
        }
        public static void DrawAsWater(SpriteBatch spriteBatch)
        {
            lock (ListOfParticles)
            {
                for (int i = 0; i < ListOfParticles.Count; i++)
                {
                    ListOfParticles[i].DrawAsBigCircle(spriteBatch);
                }

                for (int i = 0; i < ListOfParticles.Count; i++)
                {
                    ListOfParticles[i].DrawAsSmallerCircle(spriteBatch);
                }
            }

            for (int i = 0; i < ListOfMarkers.Count; i++)
            {
                ListOfMarkers[i].Draw(spriteBatch);
            }
        }

        public static void DrawText(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Assets.Font, ListOfParticles.Count.ToString(), new Vector2(5, 5), Color.LimeGreen);
            spriteBatch.DrawString(Assets.Font, ListOfMarkers.LongCount(Marker => Marker.Positive == true).ToString(), new Vector2(5, 22), Color.Red);
            spriteBatch.DrawString(Assets.Font, ListOfMarkers.LongCount(Marker => Marker.Positive == false).ToString(), new Vector2(5, 39), Color.Blue);
            EingabenAnzeige.Draw(spriteBatch);
        }
    }
}
