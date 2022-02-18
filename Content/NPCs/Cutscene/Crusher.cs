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
    public class Crusher : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crusher");
        }

        public override void SetDefaults()
        {
            npc.width = 74;
            npc.height = 10;
            npc.damage = 12;
            npc.defense = 1000;
            npc.lifeMax = 1000;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.value = 100f;
            npc.knockBackResist = 0.0f;
            npc.aiStyle = -1;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }


        public Vector2 BasePosition;
        public float HeightDescended;
        public float fallSpeed;
        public float riseStep;
        int fallSpeedCache;

        public const int TimeInGround = 80;
        public const int TimeUpTop = 50;
        public const float SPEEDOFFALL = 0.007f;
        public const int CrushBox = 16;
        public const int ChainLength = 86;

        public int HEIGHTOFFALL;
        public int ChainsNeeded;
        public float DeltaY;
        public Vector2 BottomPosition => new Vector2(BasePosition.X, BasePosition.Y + HeightDescended);
        public override void AI()
        {
            HeightDescended = MathHelper.Clamp(HeightDescended, 0, HEIGHTOFFALL);

            if (npc.ai[2] > 0) npc.ai[2]--;

            if (npc.ai[0] == 0)
            {
                HEIGHTOFFALL = LinuxTechTips.TileCheckVertical(npc.position + new Vector2(0, 32)) * 16 - (int)npc.position.Y;
                BasePosition = npc.position - new Vector2(0, HEIGHTOFFALL);
                ChainsNeeded = (HEIGHTOFFALL - 102) / 86 + 1;
            }

            npc.height = CrushBox;

            npc.ai[0]++;

            if (npc.ai[1] == 0)
            {
                if (npc.ai[2] == 0)
                {
                    fallSpeed++;
                    HeightDescended += SPEEDOFFALL * fallSpeed * fallSpeed;
                }
            }

            if (npc.ai[1] == 1)
            {
                if (npc.ai[2] == 0)
                {
                    riseStep += 0.01f;
                    HeightDescended = MathHelper.SmoothStep(HEIGHTOFFALL, 0, riseStep);
                }
                else if (npc.ai[2] == TimeInGround - 1)
                {
                    Main.LocalPlayer.GetModPlayer<LinuxPlayer>().ScreenShake = 20;
                    LinuxTechTips.Particles.SetSpawningModules(new SpawnRandomly(1f));
                    for(int i = 0; i<80; i++)
                    LinuxTechTips.Particles.SpawnParticles(
                      npc.position + new Vector2(Main.rand.Next(npc.width), -SPEEDOFFALL * fallSpeedCache * fallSpeedCache),
                      Vector2.One.RotatedBy(Main.rand.NextFloat(-3f, 3f)) * 0.1f + new Vector2(0, Main.rand.NextFloat(0f, -0.5f)), 8,
                      Color.Lerp(Color.White, Color.Gray, Main.rand.NextFloat(0f, 1f)) * Main.rand.NextFloat(0f, 0.2f),
                      new SlowDown(0.97f), new AfterImageTrail(1f), new SetShrinkSize(0.9f), new SetTimeLeft(20), new RotateTexture(0.1f));
                }
            }

            if (HeightDescended > HEIGHTOFFALL && npc.ai[1] == 0)
            {
                Reset(1);
            }

            if (HeightDescended < 0.03f && npc.ai[1] == 1)
            {
                Reset(0);
            }

            npc.position.Y = BasePosition.Y + HeightDescended + HEIGHTOFFALL - CrushBox;
            npc.velocity.Y = HeightDescended - DeltaY;
            DeltaY = HeightDescended;


        }

        public void Reset(int state)
        {
            npc.ai[1] = state;
            npc.ai[2] = TimeInGround;
            fallSpeed = 0;
            riseStep = 0;
            fallSpeedCache = (int)fallSpeed;
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Asset.GetTexture("NPCs/Cutscene/Crusher");
            Texture2D texChain = Asset.GetTexture("NPCs/Cutscene/CrusherChain");
            //spriteBatch.Draw(Main.magicPixel, new Vector2(BasePosition.X, BasePosition.Y).ForDraw() + new Vector2(0, HEIGHTOFFALL), new Rectangle(0, HEIGHTOFFALL - (int)HeightDescended, npc.width, (int)HeightDescended), Color.Red);
            spriteBatch.Draw(tex, new Vector2(BasePosition.X, BasePosition.Y).ForDraw() + new Vector2(0, HEIGHTOFFALL), new Rectangle(0, 102 - (int)HeightDescended, npc.width, (int)HeightDescended), lightColor);
            for (int i = 0; i < ChainsNeeded; i++)
            {
                spriteBatch.Draw(texChain, new Vector2(BasePosition.X, BasePosition.Y).ForDraw() + new Vector2(0, HEIGHTOFFALL), new Rectangle(0, 102 + 86 * (i + 1) - (int)HeightDescended, npc.width, (int)HeightDescended - 102 - 86 * i), lightColor);
            }

            return false;
        }
    }
}
