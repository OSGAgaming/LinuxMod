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
    public class RapierProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rapier Projectile");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.penetrate = 20;
            projectile.timeLeft = 10000;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.magic = true;
            projectile.knockBack = 2f;
        }

        public override void AI()
        {
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
           // LUtils.DrawRectangle(projectile.position.ForDraw(), projectile.width, projectile.height, Color.White, 3);
            return false;
        }
    }
}