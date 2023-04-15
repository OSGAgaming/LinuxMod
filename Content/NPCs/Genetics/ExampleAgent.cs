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

namespace LinuxMod.Content.NPCs.Genetics
{
    public class ExampleAgent : Agent
    {
        public float speed => 1f;

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

            sight.Add(Math.Min(200,npc.position.X - Main.LocalPlayer.position.X) / 10f);
            sight.Add(Math.Min(200, npc.position.Y - Main.LocalPlayer.position.Y) / 10f);
            sight.Add(npc.velocity.X);
            sight.Add(npc.velocity.Y);

            return sight;
        }

        public override void Response(NetLayer output)
        {
            float[] controlChances = output.ComputeSoftMaxedLayer();

            int index = 0;
            float r = Main.rand.NextFloat(1);

            while (r > 0)
            {
                r -= controlChances[index++];
            }

            for (int i = 0; i < output.nodes.Count; i++)
            {
                //Main.NewText();
            }

            index--;

            index = (int)output.Max();

            if (index == 0) npc.velocity.X += speed;
            else if (index == 1) npc.velocity.X -= speed;
            else if (index == 2) npc.velocity.Y += speed;
            else if (index == 3) npc.velocity.Y -= speed;
            else if (index == 4) npc.velocity += Vector2.Normalize(new Vector2(1, 1)) * speed;
            else if (index == 5) npc.velocity += Vector2.Normalize(new Vector2(1, -1)) * speed;
            else if (index == 6) npc.velocity += Vector2.Normalize(new Vector2(-1, 1)) * speed;
            else if (index == 7) npc.velocity += Vector2.Normalize(new Vector2(-1, -1)) * speed;
            //else if (index == 8) npc.velocity += Vector2.Zero;

            npc.velocity *= 0.8f;

            npc.rotation += ((npc.velocity.ToRotation() + MathHelper.PiOver2) - npc.rotation) / 16f;
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
