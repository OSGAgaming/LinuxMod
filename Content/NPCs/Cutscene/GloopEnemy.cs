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
    public class GloopEnemy : ModNPC
    {
        private int MetaBallAccuracy = 20;
        private int Dimensions = 30;
        private int DimensionDeviation = 6;
        private List<GloopMetaBall> MetaBalls = new List<GloopMetaBall>();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gloop Enemy");
        }

        public override void SetDefaults()
        {
            npc.width = 10;
            npc.height = 10;
            npc.damage = 12;
            npc.defense = 10;
            npc.lifeMax = 13;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.value = 100f;
            npc.knockBackResist = 0.3f;
            npc.aiStyle = -1;
            npc.behindTiles = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
        }


        private float t => Main.GameUpdateCount;
        private float wallConnection;
        public void UpdateMetaBalls()
        {
            wallConnection = 0;
            foreach (GloopMetaBall ball in MetaBalls)
            {
                ball.Update();

                bool inTile = Framing.GetTileSafely(ball.position.ToTileCoordinates()).active();
                if (inTile) wallConnection++;
                if (!inTile) ball.velocity += (npc.Center - ball.position) / ball.SHMFactor - ball.velocity / ball.Dampening;
                else ball.velocity += (npc.Center - ball.position) / (ball.SHMFactor / 10) - ball.velocity / (ball.Dampening / 5);
                ball.dimensions = ball.origDims + new Vector2((float)Math.Sin(t / ball.SquishOsc), (float)Math.Cos(t / ball.SquishOsc)) * 4;
                foreach (GloopMetaBall incumbentball in MetaBalls)
                {
                    if (!ball.Equals(incumbentball))
                    {
                        float sumOfCenters = ball.dimensions.X + incumbentball.dimensions.X;
                        float dist = Vector2.Distance(ball.position, incumbentball.position);
                        float coalesceFactor = sumOfCenters - dist;
                        if (coalesceFactor > 0)
                        {
                            Vector2 reslove = Vector2.Normalize(ball.position - incumbentball.position) * coalesceFactor;
                            if (!inTile) ball.position += reslove / 2000f;
                            else ball.position += reslove / 1000f;
                        }
                    }
                }
            }
            wallConnection /= MetaBallAccuracy;
        }

        public void DrawMetaBalls(SpriteBatch sb)
        {
            foreach (GloopMetaBall ball in MetaBalls)
            {
                float Width = 1;
                ScreenMapPass.Instance.GetMap("SGMap").DrawToBatchedTarget((SpriteBatch sba) =>
                {
                    ball.Draw(sb);
                });

                AdditiveCalls.Instance.AddCall((SpriteBatch sba) =>
                {
                    Texture2D tex = LinuxTechTips.RadialMask;
                    sb.Draw(tex, ball.position.ForDraw(), tex.Bounds, Color.LimeGreen * 0.2f * Math.Min(1, Width), 0f, tex.TextureCenter(), Math.Min(1, Width) * 0.52f, SpriteEffects.None, 0f);
                });
            }
        }

        public override void AI()
        {
            Lighting.AddLight(npc.Center, new Vector3(0,0.2f,0));

            if (npc.ai[0] == 0)
            {
                for (int i = 0; i < MetaBallAccuracy; i++)
                {
                    GloopMetaBall ball = new GloopMetaBall(new Vector2(Dimensions + Main.rand.NextFloat(-DimensionDeviation, DimensionDeviation)));
                    ball.position = npc.position + new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10));
                    ball.SHMFactor = Main.rand.NextFloat(200, 250);
                    ball.Dampening = Main.rand.NextFloat(10, 20);
                    ball.SquishOsc = Main.rand.NextFloat(10, 20);
                    MetaBalls.Add(ball);
                }
            }
            npc.TargetClosest(true);
            Player target = Main.player[npc.target];
            Point pL = npc.Center.ToTileCoordinates();
            Tile tileR = Framing.GetTileSafely(new Point(pL.X + 1, pL.Y));
            Tile tileL = Framing.GetTileSafely(new Point(pL.X - 1, pL.Y));
            Tile tileD = Framing.GetTileSafely(new Point(pL.X, pL.Y + 1));
            bool isLeft = tileL.active();
            bool isRight = tileR.active();
            if (npc.life <= 0)
            {
                MetaBalls.Clear();
            }
            UpdateMetaBalls();

            bool canHit = Collision.CanHitLine(npc.Center, 1, 1, target.Center, 1, 1);
            if (tileD.active() || Math.Abs(npc.velocity.X) <= 0.01f)
            {
                npc.velocity.X = Vector2.Normalize(target.Center - npc.Center).X * 2;
            }
            else
            {
                npc.velocity.X *= 0.6f;
            }
            if ((isLeft && npc.velocity.X < 0.1f) || (isRight && npc.velocity.X > -0.1f) || (npc.velocity.X == 0 && (isLeft || isRight)))
            {
                if (target.Center.Y < npc.Center.Y || canHit)
                {
                    npc.velocity.Y = -2f;
                }
            }
            npc.ai[0]++;
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawMetaBalls(spriteBatch);
            Vector2 nP = npc.position.ForDraw();
            //LUtils.DrawRectangle(new Rectangle((int)nP.X, (int)nP.Y, 10,10), Color.White, 3);
            return false;
        }
    }
}
