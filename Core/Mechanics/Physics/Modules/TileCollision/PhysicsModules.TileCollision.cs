using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Verlet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    [Needs(typeof(PolygonModule))]
    public class TileCollisionModule : Module
    {
        public Polygon Polygon;
        public CollisionInfo collisionInfo;
        private int SampleSize = 64;
        private int index = -1; 
        public override void Update()
        {
            Polygon = new Polygon(Object.GetModule<PolygonModule>().Polygon.points, Object.Center);
            VerletModule Verlet = Object.GetModule<VerletModule>();
            collisionInfo.d = Vector2.Zero;

            Vector2 TL = Object.Center - new Vector2(SampleSize) / 2;
            int TileSampleSize = SampleSize / 16;
            Point tTL = TL.ToTileCoordinates();

            for (int i = 0; i < TileSampleSize; i++)
            {
                for (int j = 0; j < TileSampleSize; j++)
                {
                    Tile tile = Framing.GetTileSafely(tTL.X + i, tTL.Y + j);
                    if (tile.active() && Main.tileSolid[tile.type])
                    {
                        Vector2 C = new Vector2(tTL.X + i, tTL.Y + j) * 16;

                        Polygon poly = Polygon.RectToPoly(new Rectangle((int)C.X, (int)C.Y, 16, 16));
                        CollisionInfo Info = LinuxCollision.SAT(Polygon, poly);

                        collisionInfo.d += Info.d;

                        if (Info.d != Vector2.Zero)
                        {
                            poly.Draw();
                            int indexOfClosest = -1;
                            float lowestLength = float.MaxValue;
                            for (int a = 0; a < Verlet.indexes.Count; a++)
                            {
                                float length = Vector2.Dot(LinuxMod.GetLoadable<VerletSystem>().GetPoint(Verlet.indexes[a]).point - poly.Center, Info.Axis);
                                if (Math.Abs(length) < lowestLength)
                                {
                                    indexOfClosest = Verlet.indexes[a];
                                    lowestLength = Math.Abs(length);
                                }
                            }

                            index = indexOfClosest;
                            LinuxMod.GetLoadable<VerletSystem>().GetPoint(indexOfClosest).point -= Info.Depth * Info.Axis * 0.5f;
                            Object.Velocity = Vector2.Zero;
                            Vector2 v = LinuxMod.GetLoadable<VerletSystem>().GetPoint(indexOfClosest).point;
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
        }
        public TileCollisionModule()
        {
            GetHost<TileCollisionModule>().AddObject(this);
        }
    }
}