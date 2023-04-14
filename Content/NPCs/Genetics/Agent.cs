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

namespace LinuxMod.Content.NPCs.Genetics
{
    public class Agent : ModNPC
    {
        public BaseNeuralNetwork network;
        public float speed => 0.3f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gentic Agent");
        }

        public override void SetDefaults()
        {
            npc.width = 10;
            npc.height = 10;
            npc.damage = 12;
            npc.defense = 10;
            npc.lifeMax = 13;
            npc.aiStyle = -1;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
        }

        public override void AI()
        {
            if (npc.ai[0] == 0)
            {
                network = new BaseNeuralNetwork(9)
                    .AddLayer<SigmoidActivationFunction>(12)
                    .AddLayer<SigmoidActivationFunction>(12)
                    .SetOutput<LinearActivationFunction>(4)
                    .GenerateWeights(() => Main.rand.NextFloat(-1,1));

                npc.ai[0] = 1;
            }

            Point tilePos = npc.Center.ToTileCoordinates();
            List<float> sight = new List<float>();

            for(int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    Tile t = Framing.GetTileSafely(new Point(tilePos.X + i, tilePos.Y + j));
                    sight.Add((t.active() && Main.tileSolid[t.type]) ? 1 : 0);
                }
            }

            network.UpdateNetwork(sight.ToArray());

            Response((int)network.Outputs.Max());
            npc.velocity *= 0.95f;
        }

        public void Response(int output)
        {
            Main.NewText(output);

            if (output == 0) npc.velocity.X += speed;
            else if (output == 1) npc.velocity.X -= speed;
            else if (output == 2) npc.velocity.Y += speed;
            else if (output == 3) npc.velocity.Y -= speed;
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 nP = npc.position.ForDraw();
            LinuxTechTips.DrawRectangle(new Rectangle((int)nP.X, (int)nP.Y, 10,10), Color.White, 3);

            network.Draw(spriteBatch, Main.screenPosition.ForDraw() + new Vector2(50));

            return false;
        }
    }
}
