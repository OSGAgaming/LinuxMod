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

namespace LinuxMod.Content.NPCs.Cutscene
{
    public class BallHazard : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ball Hazard");
        }

        public override void SetDefaults()
        {
            npc.width = 50;
            npc.height = 50;
            npc.damage = 12;
            npc.defense = 1000;
            npc.lifeMax = 1000;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.value = 100f;
            npc.knockBackResist = 0.3f;
            npc.aiStyle = -1;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        private int LastIndex;
        Vector2 BasePoint;
        float textAlpha;

        private const int TOTAL_LENGTH = 500;
        private const float TIME_OF_WINDUP = 400;
        private const float TIME_BETWEEM_WINDUPS = 700;
        private const int CHAIN_COUNT = 10;

        VPoint ControlPoint => Core.LinuxMod.GetLoadable<VerletSystem>().GetPoint(LastIndex);
        public override void AI()
        {
            if (npc.ai[0] == 0)
            {
                LastIndex = Core.LinuxMod.GetLoadable<VerletSystem>().BindPoints(npc.position, CHAIN_COUNT, TOTAL_LENGTH/CHAIN_COUNT);
                ControlPoint.point = npc.Center;
            }
            else if(npc.ai[0] < TIME_OF_WINDUP)
            {
                ControlPoint.point = npc.Center;
                BasePoint = npc.Center;
            }
            else
            {
                npc.Center = ControlPoint.point;
                if (npc.ai[0] % TIME_BETWEEM_WINDUPS < TIME_OF_WINDUP)
                {
                    Core.LinuxMod.GetLoadable<VerletSystem>().GetPoint(LastIndex - CHAIN_COUNT + 1).point.Y -= TOTAL_LENGTH/ TIME_OF_WINDUP;
                    if (Vector2.DistanceSquared(Main.LocalPlayer.Center, npc.Center) < 200 * 200)
                    {
                        textAlpha += (1 - textAlpha) / 4f;
                        Vector2 v = npc.Center + new Vector2(0, 45);
                        if (Main.LocalPlayer.controlUp)
                        {
                            Main.LocalPlayer.Center += (v - Main.LocalPlayer.Center) / 4f;
                            Main.LocalPlayer.fullRotation = (BasePoint - Main.LocalPlayer.Center).ToRotation() + 1.57f;
                            Main.LocalPlayer.velocity = Vector2.Zero;
                        }
                        else
                        {
                            Main.LocalPlayer.fullRotation *= 0.98f;
                        }
                    }
                    else
                    {
                        textAlpha += (0 - textAlpha) / 16f;
                        Main.LocalPlayer.fullRotation *= 0.98f;
                    }
                }
                else if (npc.ai[0] % TIME_BETWEEM_WINDUPS < TIME_OF_WINDUP + 50)
                {
                    Main.LocalPlayer.fullRotation *= 0.98f;
                    textAlpha += (0 - textAlpha) / 16f;
                    ControlPoint.point = BasePoint;
                    npc.Center = ControlPoint.point;
                    Core.LinuxMod.GetLoadable<VerletSystem>().GetPoint(LastIndex - CHAIN_COUNT + 1).point = BasePoint;
                }
                
            }

            npc.ai[0]++;
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            ControlPoint.oldPoint -= projectile.velocity;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 v = npc.Center + new Vector2(0, 20);
            LinuxTechTips.UITextToCenter("Bazinga", lightColor * textAlpha, v.ForDraw(), 1);

            Point p = npc.position.ForDraw().ToPoint();
            spriteBatch.Draw(Main.magicPixel, new Rectangle(p.X, p.Y, 50, 50), lightColor);
            return false;
        }
    }
}
