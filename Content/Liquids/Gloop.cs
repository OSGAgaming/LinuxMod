using LinuxMod.Content.Projectiles.Cutscene;
using LinuxMod.Content.Tiles;
using LinuxMod.Core.Assets;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class Gloop : Liquid
    {
        private const float IMPULSE = 0.15f;
        GloopWallReflection WR;
        public override void OnLoad()
        {
            WR = new GloopWallReflection(this);
        }
        public override void OnDraw(SpriteBatch sb)
        {
            WR.Draw(sb);
        }
        public override void OnUpdate() 
        {
            WR.Update();
            for (int i = 0; i < 200; i++)
            {
                Projectile p = Main.projectile[i];

                if (p.type == ModContent.ProjectileType<WallWellWater>())
                {
                    Vector2 pos = frame.Location.ToVector2();
                    if(p.position.X > pos.X && p.position.X < pos.X + frame.Width)
                    {
                        float perc = (p.position.X + 32 - pos.X) / frame.Width;
                        float perc2 = (p.position.X + 28 - pos.X) / frame.Width;
                        float perc3 = (p.position.X + 36 - pos.X) / frame.Width;
                        if (Main.rand.Next(2) == 0)
                        {
                            SplashPerc(perc, Vector2.UnitY * Main.rand.NextFloat(IMPULSE,IMPULSE*2));

                            SplashPerc(perc2, Vector2.UnitY * Main.rand.NextFloat(IMPULSE, IMPULSE * 2));

                            SplashPerc(perc3, Vector2.UnitY * Main.rand.NextFloat(IMPULSE, IMPULSE * 2));

                            Vector2 splashPoint = new Vector2(p.position.X + 25, frame.Y + 10);

                            LUtils.Particles.SetSpawningModules(new SpawnRandomly(0.13f));
                            for (int a = 0; a < 5; a++)
                                LUtils.Particles.SpawnParticles(
                                splashPoint, 
                                new Vector2(Main.rand.NextFloat(-0.8f, 0.8f), Main.rand.NextFloat(-0.5f, -0.1f)), 10,
                                Color.White * 0.2f, 
                                new SlowDown(0.96f), 
                                new RotateTexture(Main.rand.NextFloat(-0.03f, 0.03f)),
                                new SetMask(Asset.GetTexture("Masks/RadialGradient")), new AfterImageTrail(1f), 
                                new SetLighting(Color.White.ToVector3(), 0.1f));
                        }
                    }
                }
            }
        }
    }
}


