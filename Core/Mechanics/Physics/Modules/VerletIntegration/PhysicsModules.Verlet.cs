using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Verlet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    [Needs(typeof(PolygonModule))]
    public class VerletModule : Module
    {
        public Vector2[] Points;

        public List<int> indexes = new List<int>();

        public override void Update()
        {
            Vector2 ConcentricPoint = Vector2.Zero;

            VerletSystem system = LinuxMod.verletSystem;

            PolygonModule c = Object.GetModule<PolygonModule>();
            //PhysicsCollision p = Object.GetModule<PhysicsCollision>();
            //TileCollision t = Object.GetModule<TileCollision>();

            for (int i = 0; i < c.Polygon.points.Length; i++)
            {
                //system.GetPoint(indexes[i]).point -= p.collisionInfo.d;
                //system.GetPoint(indexes[i]).oldPoint -= p.collisionInfo.d;

                //system.GetPoint(indexes[i]).point -= t.collisionInfo.d;
                //system.GetPoint(indexes[i]).oldPoint -= t.collisionInfo.d;

                system.GetPoint(indexes[i]).point += Object.Velocity;
                system.GetPoint(indexes[i]).oldPoint += Object.Velocity;
            }

            for (int i = 0; i < c.Polygon.points.Length; i++)
            {
                ConcentricPoint += system.GetPoint(indexes[i]).point;

                Vector2 relative = system.GetPoint(indexes[i]).point - Object.Center;
                c.Polygon.points[i] = relative;
            }

            ConcentricPoint /= Points.Length;
            Object.Center = ConcentricPoint;
        }

        public override void Load()
        {
            PolygonModule c = Object.GetModule<PolygonModule>();
            Points = c.Polygon.points;
            VerletSystem system = LinuxMod.verletSystem;

            for (int i = 0; i < c.Polygon.points.Length; i++)
            {
                int a = system.CreateVerletPoint(c.Polygon.varpoints[i], false, true);
                system.GetPoint(a).oldPoint = system.GetPoint(a).oldPoint + new Vector2(Main.rand.NextFloat(-4, 4), 0);
                indexes.Add(a);

                if (i > 0)
                {
                    system.BindPoints(a, a - 1);
                    if(i > 1)
                    system.BindPoints(a, a - 2);
                }

            }
            system.BindPoints(indexes[indexes.Count - 1], indexes[0]);
        }

        public override void Draw(SpriteBatch sb)
        {
            VerletSystem system = LinuxMod.verletSystem;
            PolygonModule c = Object.GetModule<PolygonModule>();

            for (int i = 0; i < c.Polygon.points.Length; i++)
            {
                Vector2 v1 = system.GetPoint(indexes[i]).point.ForDraw();

                if (i > 0)
                {
                    Vector2 v2 = system.GetPoint(indexes[i - 1]).point.ForDraw();

                    LUtils.DrawLine(v2, v1, Color.Red, 2);

                    if (i == c.Polygon.points.Length - 1)
                        LUtils.DrawLine(system.GetPoint(indexes[0]).point.ForDraw(), v1, Color.Red, 2);
                }
            }

        }
        public VerletModule()
        {
            GetHost<VerletModule>().AddObject(this);
        }
    }
}