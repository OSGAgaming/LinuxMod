using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Content.NPCs.Genetics;

namespace LinuxMod.Core.Mechanics
{
    public class ExampleAgent : Agent
    {
        public float speed => 4f;
        public float angularSpeed => 0.3f;

        public float bearings = 0f;
        public int Type;

        List<Vector2> previous = new List<Vector2>();

        public override void OnSpawn()
        {
            width = 16;
            height = 16;
            Type = ModContent.NPCType<ExampleAgentNPC>();
        }

        public float DistanceReached(Vector2 direction, float maxDist, float precision)
        {
            Vector2 norm = Vector2.Normalize(direction);
            bool canHit = Collision.CanHitLine(Center, 1, 1, Center + norm * maxDist, 1, 1);

            if (canHit)
            {
                return maxDist;
            }
            else
            {
                bool hasHit = false;
                float progress = 0;
                while (!hasHit && progress < maxDist)
                {
                    progress += precision;

                    Point p = (Center + norm * progress).ToTileCoordinates();

                    Tile t = Framing.GetTileSafely(p);
                    hasHit = t.active() && Main.tileSolid[t.type];
                }

                progress -= precision;

                return progress / maxDist;
            }
        }

        public override List<float> FeedInputs()
        {
            List<float> sight = new List<float>();

            float sightDistance = 64;
            float precision = 16;

            sight.Add(DistanceReached(Vector2.UnitY, sightDistance, precision));
            sight.Add(DistanceReached(-Vector2.UnitY, sightDistance, precision));
            sight.Add(DistanceReached(Vector2.UnitX, sightDistance, precision));
            sight.Add(DistanceReached(-Vector2.UnitX, sightDistance, precision));

            sight.Add(DistanceReached(-Vector2.One, sightDistance, precision));
            sight.Add(DistanceReached(Vector2.One, sightDistance, precision));
            sight.Add(DistanceReached(-Vector2.One * new Vector2(-1,1), sightDistance, precision));
            sight.Add(DistanceReached(Vector2.One * new Vector2(-1, 1), sightDistance, precision));

            sight.Add((position.X - Main.LocalPlayer.position.X) / 200f);
            sight.Add((position.Y - Main.LocalPlayer.position.Y) / 200f);
            sight.Add(rotation % MathHelper.TwoPi);

            return sight;
        }

        public override void Response(float[] output)
        {
            float value = output.Max();
            int maxIndex = Array.IndexOf(output, value);

            if (maxIndex == 0) bearings += 0;
            else if (maxIndex == 1) bearings -= angularSpeed;
            else if (maxIndex == 2) bearings += angularSpeed;

            velocity = Vector2.UnitX.RotatedBy(bearings) * speed;

            rotation = velocity.ToRotation() + MathHelper.PiOver2;
            velocity = Collision.TileCollision(position, velocity, width, height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 nP = position.ForDraw();
            LinuxTechTips.DrawRectangle(new Rectangle((int)nP.X, (int)nP.Y, width, height), Color.White, 1);

            Texture2D tex = Main.npcTexture[Type];
            spriteBatch.Draw(tex, Center.ForDraw(), tex.Bounds, Color.White, rotation, tex.TextureCenter(), 1f, SpriteEffects.None, 0f);
        }
    }
}