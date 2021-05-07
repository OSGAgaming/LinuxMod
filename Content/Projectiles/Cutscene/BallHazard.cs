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
    public class BallHazard : ModProjectile
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


        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Point p = projectile.position.ForDraw().ToPoint();
            spriteBatch.Draw(Main.magicPixel, new Rectangle(p.X, p.Y, 50,50), Color.White);
            return false;
        }
    }
}