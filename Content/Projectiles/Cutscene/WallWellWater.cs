using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

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
            if(projectile.ai[0] == 0)
            {
                WaterPositionCache.Positions.Add(projectile.position);
                projectile.ai[0] = 1;
            }
        }

        public override void Kill(int timeLeft)
        {
            WaterPositionCache.Positions.Remove(projectile.position);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
           /* ScreenMapPass.Instance.GetMap("Sewer").DrawToTarget((SpriteBatch sb) =>
            {
                Vector2 v = projectile.position.ForDraw();
                Point p1 = v.ToPoint();
                Point p2 = new Point(100, 100);
                sb.Draw(Main.magicPixel, new Rectangle(p1.X, p1.Y, p2.X, p2.Y), Color.Green);
            });

            ScreenMapPass.Instance.GetMap("Sewer").ApplyShader();*/
            return false;
        }
    }
}