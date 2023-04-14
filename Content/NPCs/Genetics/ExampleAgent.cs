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
        public float speed => 0.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gentic Agent");
        }

        public override void SetDefaults()
        {
            npc.width = 10;
            npc.height = 10;
            npc.lifeMax = 13;
            npc.aiStyle = -1;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
        }

        public override List<float> FeedInputs()
        {
            List<float> sight = new List<float>();
            Point tilePos = npc.Center.ToTileCoordinates();

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    Tile t = Framing.GetTileSafely(new Point((int)MathHelper.Clamp(tilePos.X, 1, Main.maxTilesX) + i, (int)MathHelper.Clamp(tilePos.Y, 1, Main.maxTilesY) + j));
                    sight.Add((t.active() && Main.tileSolid[t.type]) ? 1 : 0);
                }
            }
            sight.Add(npc.position.X - Main.LocalPlayer.position.X);
            sight.Add(npc.position.Y - Main.LocalPlayer.position.Y);
            sight.Add(npc.velocity.X);
            sight.Add(npc.velocity.Y);

            return sight;
        }

        public override void Response(NetLayer output)
        {
            float[] controlChances = output.ComputeSoftMaxedLayer();

            int index = 0;
            float r = Main.rand.NextFloat(1);

            while(r > 0)
            {
                r -= controlChances[index++];
            }

            index--;
            if (index == 0) npc.velocity.X += speed;
            else if (index == 1) npc.velocity.X -= speed;
            else if (index == 2) npc.velocity.Y += speed;
            else if (index == 3) npc.velocity.Y -= speed;

            npc.velocity *= 0.98f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 nP = npc.position.ForDraw();
            LinuxTechTips.DrawRectangle(new Rectangle((int)nP.X, (int)nP.Y, 10, 10), Color.White, 3);

            network?.Draw(spriteBatch, Main.screenPosition.ForDraw() + new Vector2(50));

            return false;
        }
    }
}
