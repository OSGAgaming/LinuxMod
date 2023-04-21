using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using LinuxMod.Core.Helper;
using Terraria.ModLoader;
using LinuxMod.Core;
using LinuxMod.Core.Assets;
using Terraria.ID;
using LinuxMod.Core.Mechanics.Verlet;
using System.Linq;
using LinuxMod.Core.Mechanics.ScreenMap;

namespace LinuxMod.Content.NPCs.Genetics
{
    public class ExampleAgentNPC : AgentNPC
    {
        public float speed => 4f;
        public float angularSpeed => 0.3f;

        public float rotation = 0f;

        List<Vector2> previous = new List<Vector2>();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goobloid");
        }

        public override void SetDefaults()
        {
            npc.width = 16;
            npc.height = 16;
            npc.lifeMax = 13;
            npc.aiStyle = -1;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
        }

        public int Map(bool b) => b ? 0 : 2;

        public override List<float> FeedInputs()
        {
            if(previous.Count <= 0)
            {
                previous.Add(npc.Center);
            }
            List<float> sight = new List<float>();
            Point tilePos = npc.Center.ToTileCoordinates();

            float sightDistance = 64;

            sight.Add(Map(Collision.CanHitLine(npc.Center, 1, 1, npc.Center - Vector2.UnitY * sightDistance, 1, 1)));
            sight.Add(Map(Collision.CanHitLine(npc.Center, 1, 1, npc.Center + Vector2.UnitY * sightDistance, 1, 1)));
            sight.Add(Map(Collision.CanHitLine(npc.Center, 1, 1, npc.Center - Vector2.UnitX * sightDistance, 1, 1)));
            sight.Add(Map(Collision.CanHitLine(npc.Center, 1, 1, npc.Center + Vector2.UnitX * sightDistance, 1, 1)));

            sight.Add(Map(Collision.CanHitLine(npc.Center, 1, 1, npc.Center + Vector2.One * sightDistance, 1, 1)));
            sight.Add(Map(Collision.CanHitLine(npc.Center, 1, 1, npc.Center - Vector2.One * sightDistance, 1, 1)));
            sight.Add(Map(Collision.CanHitLine(npc.Center, 1, 1, npc.Center + Vector2.One * -Vector2.UnitX * sightDistance, 1, 1)));
            sight.Add(Map(Collision.CanHitLine(npc.Center, 1, 1, npc.Center - Vector2.One * -Vector2.UnitX * sightDistance, 1, 1)));
            
            sight.Add((npc.position.X - Main.LocalPlayer.position.X) / 200f);
            sight.Add((npc.position.Y - Main.LocalPlayer.position.Y) / 200f);
            sight.Add(rotation % MathHelper.TwoPi);

            return sight;
        }

        public override void Response(float[] output)
        {
            float value = output.Max();
            int maxIndex = Array.IndexOf(output, value);

            if (maxIndex == 0) rotation += 0;
            else if (maxIndex == 1) rotation -= angularSpeed;
            else if (maxIndex == 2) rotation += angularSpeed;

            npc.velocity = Vector2.UnitX.RotatedBy(rotation) * speed;

            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 nP = npc.position.ForDraw();
            LinuxTechTips.DrawRectangle(new Rectangle((int)nP.X, (int)nP.Y, npc.width, npc.height), Color.White, 1);

            float sightDistance = 64;
            Texture2D tex = Main.npcTexture[npc.type];
            spriteBatch.Draw(tex, npc.Center.ForDraw(), tex.Bounds, Color.White, npc.rotation, tex.TextureCenter(), 1f, SpriteEffects.None, 0f);
            bool hasHit = Collision.CanHitLine(npc.Center, 1, 1, npc.Center - Vector2.UnitY * sightDistance, 1, 1);
            //network?.Draw(spriteBatch, Main.screenPosition.ForDraw() + new Vector2(200));

            return false;
        }
    }
}
