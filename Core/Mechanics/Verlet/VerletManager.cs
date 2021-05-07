using EEMod.Extensions;
using LinuxMod.Core.Helper.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core.Mechanics.Verlet
{
    public class VerletSystem
    {
        private int RENDERDISTANCE => 2000;

        private readonly float _gravity = 0.4f;
        private readonly float _airResistance = 0.999f;

        public List<Stick> Sticks = new List<Stick>();
        public List<Point> Points = new List<Point>();

        public int CreateVerletPoint(Vector2 pos, bool isStatic = false)
        {
            Points.Add(new Point(pos, pos - new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), isStatic));

            return Points.Count - 1;
        }

        public void ClearPoints()
        {
            Points.Clear();
            Sticks.Clear();
        }

        public void BindPoints(int a, int b, int Length = 10, Texture2D tex = null) => Sticks.Add(new Stick(new int[] {a, b}, Length, tex));
        
        public void Update()
        {
            UpdatePoints();

            for (int i = 0; i < 2; i++)
            {
                UpdateSticks();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            RenderSticks();
        }

        private void UpdateSticks()
        {
            for (int i = 0; i < Sticks.Count; i++)
            {
                Stick stick = Sticks[i];
                if ((Main.LocalPlayer.Center - (Points[stick.StickPoints[0]].point + Points[stick.StickPoints[1]].point) / 2f).LengthSquared() < RENDERDISTANCE * RENDERDISTANCE)
                {
                    Point p1 = Points[stick.StickPoints[0]];
                    Point p2 = Points[stick.StickPoints[1]];
                    float dx = p2.point.X - p1.point.X;
                    float dy = p2.point.Y - p1.point.Y;
                    float currentLength = (float)Math.Sqrt(dx * dx + dy * dy);
                    float deltaLength = currentLength - stick.Length;
                    float perc = deltaLength / currentLength * 0.5f;
                    float offsetX = perc * dx;
                    float offsetY = perc * dy;

                    if (!p1.isStatic)
                    {
                        Points[stick.StickPoints[0]].point.X += offsetX;
                        Points[stick.StickPoints[0]].point.Y += offsetY;
                    }

                    if (!p2.isStatic)
                    {
                        Points[stick.StickPoints[1]].point.X -= offsetX;
                        Points[stick.StickPoints[1]].point.Y -= offsetY;
                    }
                }
            }
        }

        private void UpdatePoints()
        {
            for (int i = 0; i < Points.Count; i++)
            {
                if (!Points[i].isStatic && (Main.LocalPlayer.Center - Points[i].point).LengthSquared() < RENDERDISTANCE * RENDERDISTANCE)
                {
                    Points[i].vel.X = (Points[i].point.X - Points[i].oldPoint.X) * _airResistance;
                    Points[i].vel.Y = (Points[i].point.Y - Points[i].oldPoint.Y) * _airResistance;
                    Points[i].oldPoint.X = Points[i].point.X;
                    Points[i].oldPoint.Y = Points[i].point.Y;
                    Points[i].point.X += Points[i].vel.X;
                    Points[i].point.Y += Points[i].vel.Y;
                    Points[i].point.Y += _gravity;
                }
            }
        }

        private void RenderSticks()
        {
            for (int i = 0; i < Sticks.Count; i++)
            {
                Vector2 p1 = Points[Sticks[i].StickPoints[0]].point;
                Vector2 p2 = Points[Sticks[i].StickPoints[1]].point;
                Vector2 mid = p1 * 0.5f + p2 * 0.5f;

                LUtils.DrawLine(p1.ForDraw(), p2.ForDraw());
            }
        }
    }
}
