using EEMod.Extensions;
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core.Mechanics.Verlet
{
    public class VerletSystem : ILoadable
    {
        private int RENDERDISTANCE => 2000;

        private readonly float _gravity = 0.3f;
        private readonly float _airResistance = 0.998f;

        public List<Stick> Sticks = new List<Stick>();
        public List<VPoint> Points = new List<VPoint>();

        public int CreateVerletPoint(Vector2 pos, bool isStatic = false, bool hasGravity = true)
        {
            VPoint vp = new VPoint(pos, pos, isStatic);
            vp.hasGravity = hasGravity;
            Points.Add(vp);

            return Points.Count - 1;
        }

        public VPoint GetPoint(int i) => Points[i];
        public int CreateVerletPoint(VPoint p)
        {
            Points.Add(p);

            return Points.Count - 1;
        }
        public void ClearPoints()
        {
            Points.Clear();
            Sticks.Clear();
        }

        public void BindPoints(int a, int b, float Length = -1, Texture2D tex = null)
        {
            if(Length <= -1) Length = (Points[a].point - Points[b].point).Length();

            Sticks.Add(new Stick(new int[] { a, b }, Length, tex));
        }

        public void BindPoints(VPoint[] points, int Length = 10, Texture2D tex = null)
        {
            for(int i = 0; i < points.Length; i++)
            {
                int a = CreateVerletPoint(points[i]);
                if(i > 0)
                {
                    Sticks.Add(new Stick(new int[] { a, a - 1 }, Length, tex));
                }
            }
        }

        public int BindPoints(Vector2 startingPoint, int noOfChains, int Length = 10, Texture2D tex = null)
        {
            int returnNo = 0;

            for (int i = 0; i < noOfChains; i++)
            {
                if (i > 0)
                {
                    returnNo = CreateVerletPoint(startingPoint + new Vector2(0, Length*i), false);
                    Sticks.Add(new Stick(new int[] { returnNo, returnNo - 1 }, Length, tex));
                }
                else
                {
                    CreateVerletPoint(startingPoint, true);
                }
            }

            return returnNo;
        }

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
                    VPoint p1 = Points[stick.StickPoints[0]];
                    VPoint p2 = Points[stick.StickPoints[1]];
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
                    if(Points[i].hasGravity) Points[i].point.Y += _gravity/10f;
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
                Color color = LinuxTechTips.GetColor(mid.ToTileCoordinates());
                LinuxTechTips.DrawLine(p1.ForDraw(), p2.ForDraw(), color);
            }
        }

        public void Load()
        {

        }

        public void Unload()
        {

        }
    }
}
