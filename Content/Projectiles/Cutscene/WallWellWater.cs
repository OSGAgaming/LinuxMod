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

namespace LinuxMod.Content.Projectiles.Cutscene
{
    public static class WaterPositionCache
    {
        public static List<Vector2> Positions = new List<Vector2>();
    }
    public class WallWellWater : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wall Well Water");
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.scale = 1.2f;
            projectile.tileCollide = false;
            projectile.light = 0;
            projectile.timeLeft = 100000;
        }

        public override void AI()
        {
            ScreenMapPass.Instance.GetMap("Sewers").DrawToTarget((SpriteBatch sb) =>
            {
                Vector2 v = projectile.position + new Vector2(10, 16);
                int Y = LUtils.TileCheckVertical((int)v.X / 16, (int)v.Y / 16, 1, 20);
                float Diff = Y*16 - v.Y;
                sb.Draw(Main.magicPixel, v.ForDraw(), new Rectangle(0, 0, 20, (int)Diff - 10), Color.LightBlue);
            });
        }
        public override void Kill(int timeLeft)
        {
            WaterPositionCache.Positions.Remove(projectile.position);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }
    }
}