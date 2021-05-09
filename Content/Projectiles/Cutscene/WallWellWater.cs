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
            ScreenMapPass.Instance.GetMap("Sewers").DrawToBatchedTarget((SpriteBatch sb) =>
            {
                Vector2 v = projectile.position + new Vector2(10, 16 + 15);
                int Y = LUtils.TileCheckVertical((int)v.X / 16, (int)v.Y / 16, 1, 20);
                float Diff = Y * 16 - v.Y;
                sb.Draw(Main.magicPixel, v.ForDraw(), new Rectangle(0, 0, 20, (int)Diff - 10), Color.LightBlue);
            });

            ScreenMapPass.Instance.GetMap("Sewers").DrawToBatchedTarget((SpriteBatch sb) =>
            {
                Vector2 v = projectile.position + new Vector2(10, 16);
                int Y = LUtils.TileCheckVertical((int)v.X / 16, (int)v.Y / 16, 1, 20);
                float Diff = Y * 16 - v.Y;
                sb.Draw(Asset.GetTexture("Masks/Waterfall"), v.ForDraw(), new Rectangle(0, 0, 20, 16), Color.LightBlue);
            });

            Vector2 b = projectile.position + new Vector2(16, 26);

            LUtils.Particles.SetSpawningModules(new SpawnRandomly(0.13f));

            LUtils.Particles.SpawnParticles(
            b,
            Vector2.UnitX.RotatedBy(Main.rand.NextFloat(6.24f))*0.2f, 2,
            Color.LightBlue * 0.5f,
            new SlowDown(0.97f),
            new RotateTexture(Main.rand.NextFloat(-0.03f, 0.03f)),
            new SetMask(Asset.GetTexture("Masks/RadialGradient")), new AfterImageTrail(1f),
            new SetLighting(Color.White.ToVector3(), 0.1f),
            new AddVelocity(new Vector2(0, 0.05f)));

            LUtils.Particles.SpawnParticles(
            b,
            new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, -0.1f)), 7,
            Color.White * 0.1f,
            new SlowDown(0.97f),
            new RotateTexture(Main.rand.NextFloat(-0.03f, 0.03f)),
            new SetMask(Asset.GetTexture("Masks/RadialGradient")), new AfterImageTrail(1f),
            new SetLighting(Color.White.ToVector3(), 0.1f));

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }
    }
}