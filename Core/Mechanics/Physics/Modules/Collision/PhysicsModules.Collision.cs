using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Verlet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    [Needs(typeof(PolygonModule))]
    public class PhysicsCollisionModule : Module
    {
        public Polygon Polygon;
        public CollisionInfo collisionInfo;
        private int index = -1;
        public override void Update()
        {
            Polygon = new Polygon(Object.GetModule<PolygonModule>().Polygon.points, Object.Center);

            IList<PhysicsCollisionModule> CObjects = GetHost<PhysicsCollisionModule>().Objects;
            VerletModule Verlet = Object.GetModule<VerletModule>();
            collisionInfo.d = Vector2.Zero;

            foreach (PhysicsCollisionModule Module in CObjects)
            {
                PhysicsObject Entity = Module.Object;

                if (!Entity.Equals(Object))
                {
                    CollisionInfo Info = LinuxCollision.SAT(Polygon, Module.Polygon);

                    collisionInfo.d += Info.d;

                    if (Info.d != Vector2.Zero)
                    {
                        int indexOfClosest = -1;
                        float lowestLength = float.MaxValue;
                        for (int a = 0; a < Verlet.indexes.Count; a++)
                        {
                            float length = Vector2.Dot(LinuxMod.GetLoadable<VerletSystem>().GetPoint(Verlet.indexes[a]).point - Module.Polygon.Center, Info.Axis);
                            if (Math.Abs(length) < lowestLength)
                            {
                                indexOfClosest = Verlet.indexes[a];
                                lowestLength = Math.Abs(length);
                            }
                        }

                        index = indexOfClosest;
                        Vector2 resolve = -Info.Depth * Info.Axis * 0.5f;
                        LinuxMod.GetLoadable<VerletSystem>().GetPoint(indexOfClosest).point += resolve;
                        Object.Velocity = Vector2.Zero;
                    }
                }
            }
        }
        public PhysicsCollisionModule()
        {
            GetHost<PhysicsCollisionModule>().AddObject(this);
        }
    }
}