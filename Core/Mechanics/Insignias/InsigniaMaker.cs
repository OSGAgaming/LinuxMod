using LinuxMod.Core.Assets;
using LinuxMod.Core.Helper.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class InsigniaMaker
    {
        public List<INode> CurrentNodeCache = new List<INode>();

        public Vector2 CurrentMousePos;
        public Vector2 DeltaMouseCache;
        public bool CanDraw;
        public float Width;
        float down;

        public bool MouseState = Main.LocalPlayer.controlUseItem;
        public void Update()
        {
            bool JustLiftedMouse = MouseState && !Main.LocalPlayer.controlUseItem;
            bool JustPressedMouse = !MouseState && Main.LocalPlayer.controlUseItem;

            Player pa = Main.LocalPlayer;
            Vector2 pos = pa.position.ForDraw();

            LUtils.DrawRectangle(new Rectangle((int)pos.X, (int)pos.Y, pa.width, pa.height), Color.White, 2);

            LUtils.UITextToCenter(InsigniaHost.DebugTry, Color.White, Main.LocalPlayer.Center.ForDraw() - new Vector2(0, -50), 1);
            if (JustPressedMouse && CanDraw)
            {
                CurrentMousePos = Main.MouseScreen;
                CurrentNodeCache.Clear();
                Width = 1;
                down = 0;
            }

            if (!Main.LocalPlayer.controlUseItem)
            {
                if (Width > 0)
                {
                    Width += 0.1f + down;
                    down -= 0.02f;
                    LUtils.Particles.SetSpawningModules(new SpawnRandomly(0.1f));

                    for (int i = 0; i < CurrentNodeCache.Count - 1; i++)
                    {
                        LUtils.Particles.SpawnParticles(
                CurrentMousePos + CurrentNodeCache[i].Position + Main.screenPosition,
                Vector2.One.RotatedBy(Main.rand.NextFloat(-3f, 3f)) * 3, 3,
                Color.Yellow,
                new SlowDown(0.97f),
                new SetMask(Asset.GetTexture("Masks/RadialGradient")), new AfterImageTrail(1f),
                new SetLighting(Color.Yellow.ToVector3(), 0.1f),
                new RotateVelocity(Main.rand.NextFloat(-0.012f, 0.012f)), new SetShrinkSize(0.87f), new SetTimeLeft(10), new ZigzagMotion(5f, 3f));
                    }
                }
                else
                {
                    Width = 0;
                }
            }
            /*
            if (Main.LocalPlayer.controlUseItem && CanDraw)
            {
                LUtils.Particles.SetSpawningModules(new SpawnRandomly(1f));
                Vector2 Norm = Vector2.Normalize(DeltaMouseCache - Main.MouseScreen);

                for (int i = 0; i < 2; i++)
                    LUtils.Particles.SpawnParticles(
                    Main.MouseWorld + Norm.RotatedBy(1.57f) * 7 * (i * 2 - 1),
                    Norm.RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f)) * 3, 3,
                    Color.Yellow,
                    new SlowDown(0.97f),
                    new SetMask(Asset.GetTexture("Masks/RadialGradient")), new AfterImageTrail(1f),
                    new SetLighting(Color.Yellow.ToVector3(), 0.1f),
                    new RotateVelocity(Main.rand.NextFloat(-0.012f, 0.012f)), new SetShrinkSize(0.87f), new SetTimeLeft(10));
               
                    CurrentNodeCache.Add(new INode(Main.MouseScreen - CurrentMousePos, CurrentNodeCache.Count));
                
            }
            
            else if (JustLiftedMouse && CurrentNodeCache.Count > InsigniaHost.ACCURACY)
            {
                Insignia ins = new Insignia();
                ins.SetAndNormalizeNodes(CurrentNodeCache);
                LinuxMod.InsigniaSystem.CompareInsignias(ins);
            }*/

            ScreenMapPass.Instance.GetMap("InsigniaMap").DrawToBatchedTarget((SpriteBatch sb) =>
            {
                for (int i = 0; i < CurrentNodeCache.Count - 1; i++)
                {
                    Vector2 p = CurrentMousePos;
                    float distance = Vector2.Distance(CurrentNodeCache[i + 1].Position, CurrentNodeCache[i].Position);
                    for (float a = 0; a <= 1; a += 1 / distance)
                    {
                        LUtils.DrawCircle(p + Vector2.Lerp(CurrentNodeCache[i].Position, CurrentNodeCache[i + 1].Position, a), new Vector2((i / (float)CurrentNodeCache.Count) * 10 + 8f) * Width, Color.Lerp(Color.Black, Color.White, i / (float)CurrentNodeCache.Count));
                    }
                }
            });

            AdditiveCalls.Instance.AddCall((SpriteBatch sb) =>
            {
                for (int i = 0; i < CurrentNodeCache.Count - 1; i++)
                {
                    Vector2 p = CurrentMousePos;
                    float distance = Vector2.Distance(CurrentNodeCache[i + 1].Position, CurrentNodeCache[i].Position);
                    for (float a = 0; a <= 1; a += 1 / distance)
                    {
                        Texture2D tex = LUtils.RadialMask;
                        sb.Draw(tex, p + Vector2.Lerp(CurrentNodeCache[i].Position, CurrentNodeCache[i + 1].Position, a), tex.Bounds, Color.Yellow * 0.2f * Math.Min(1, Width), 0f, tex.TextureCenter(), (0.2f + (i / (float)CurrentNodeCache.Count) * 0.1f) * Math.Min(1, Width), SpriteEffects.None, 0f);
                    }
                }
            });

            MouseState = Main.LocalPlayer.controlUseItem;
            DeltaMouseCache = Main.MouseScreen;
            CanDraw = true;
        }

    }
}