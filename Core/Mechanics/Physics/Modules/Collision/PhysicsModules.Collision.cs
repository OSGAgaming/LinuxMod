using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    [Needs(typeof(PolygonModule))]
    public class PhysicsCollision : Module
    {
        public Polygon Polygon;
        public CollisionInfo collisionInfo;
        public override void Update()
        {
            Polygon = new Polygon(Object.GetModule<PolygonModule>().Polygon.points, Object.Center);

            IList<PhysicsCollision> CObjects = GetHost<PhysicsCollision>().Objects;
            collisionInfo.d = Vector2.Zero;

            foreach (PhysicsCollision Module in CObjects)
            {
                PhysicsObject Entity = Module.Object;

                if (!Entity.Equals(Object))
                {
                    CollisionInfo Info = LinuxCollision.SAT(Polygon, Module.Polygon);
                    //Object.Center -= Info.d;
                    collisionInfo.d += Info.d;

                    if(Info.d != Vector2.Zero)
                    {
                        Object.Velocity -= Info.d / 80f;
                        Module.Object.Velocity += Info.d / 80f;
                    }
                }
            }
        }
        public PhysicsCollision()
        {
            GetHost<PhysicsCollision>().AddObject(this);
        }
    }
}