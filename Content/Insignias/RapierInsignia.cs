using LinuxMod.Content.Projectiles.Cutscene;
using LinuxMod.Content.Tiles;
using LinuxMod.Core.Assets;
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class RapierInsignia : InsigniaAbility
    {
        public override string InsigniaName => "Rapier";

        public List<INode> CurrentNodeCache = new List<INode>();
        public List<Vector2> CurrentDirection = new List<Vector2>();
        public List<Vector2> DirectionPath = new List<Vector2>();

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 DeltaMouseCache;
        public Vector2 DeltaMouse;
        public Vector2 Follow;

        public float rotation;
        public float rotationSpeed;
        public float rotationTarget;
        public float rotationOffset;
        public float Width;
        public float down;
        public float AlphaAddon;

        public int index;

        public bool ReachedRotationTarget;

        public List<float> rotationCache = new List<float>();
        public List<Vector2> positionCache = new List<Vector2>();

        public const int MAXLENGTH = 10;
        public const int MaxSlashLength = 3;
        public int SlashCount = 0;

        protected override void DrawAlways(Player player, SpriteBatch sb)
        {
            for (int i = 0; i < DirectionPath.Count - 1; i++)
            {
                LUtils.DrawLine(DirectionPath[i], DirectionPath[i + 1], Color.Red, 3);
            }
        }
        public void UpdateCache()
        {
            rotationCache.Add(rotation);
            if (rotationCache.Count > MAXLENGTH)
            {
                rotationCache.RemoveAt(0);
            }

            positionCache.Add(Position);
            if (positionCache.Count > MAXLENGTH)
            {
                positionCache.RemoveAt(0);
            }
        }
        protected override void OnActive(Player player)
        {
            float Y = CurrentNodeCache.Last().Position.Y;

            for (int i = 0; i < projectiles.Count; i++)
            {
                Main.projectile[projectiles[i]].Center = Position + (CurrentNodeCache[i].Position - new Vector2(0, Y * 1.5f)).RotatedBy(rotation) + Main.screenPosition;
            }

            if (LinuxInput.JustClicked)
            {
                DirectionPath.Clear();
                ReachedRotationTarget = false;
            }
            if (LinuxInput.JustReleased)
            {
                index = 0;
                if (SlashCount >= MaxSlashLength)
                {
                    Deactivate();
                }

                CurrentDirection.Clear();
                for (int i = 0; i < DirectionPath.Count; i++)
                {
                    CurrentDirection.Add(DirectionPath[i]);
                }
                SlashCount++;

                if (CurrentDirection.Count > 0)
                {
                    float Itarget = (CurrentDirection.Last() - CurrentDirection.First()).ToRotation();
                    rotationTarget = rotation + (Itarget - rotation) * 1.1f;
                    rotationOffset = 1.3f;

                    if (SlashCount >= MaxSlashLength)
                    {
                        rotationOffset = 1.47f;
                    }
                }
            }
            if (player.controlUseItem)
            {
                DirectionPath.Add(Main.MouseScreen);
            }
            else
            {
                if (CurrentDirection.Count > 0)
                {
                    Follow = CurrentDirection[index];


                    float dist = Vector2.DistanceSquared(Position, CurrentDirection[index]);
                    if (dist < 100 * 100 && index < CurrentDirection.Count - 3)
                    {
                        index += 3;
                    }

                }
            }

            if (!ReachedRotationTarget)
            {
                rotationSpeed += (rotationTarget + 1.57f - rotation) / 90f - rotationSpeed / 5f;
                rotationOffset *= 0.98f;
            }

            UpdateCache();

            Maker.CanDraw = false;
            DeltaMouse = DeltaMouseCache - Main.MouseWorld;
            //rotation += (DeltaMouse.X/4f - rotation)/50f;
            if (rotationOffset > 1)
            {
                rotation += rotationSpeed - rotationSpeed * rotationOffset;
                if (SlashCount >= MaxSlashLength)
                {
                    AlphaAddon = (float)Math.Sin((1.47f - rotationOffset) * (Math.PI / 0.47f)) * 0.4f;
                }
                if (CurrentDirection.Count > 0)
                    Position += (CurrentDirection[0] - Position) / 20f;
            }
            else
            {
                bool Last = SlashCount >= MaxSlashLength;
                if (CurrentDirection.Count > 0)
                {
                    float indexBias = ((index * 3f) / CurrentDirection.Count + .5f);
                    float increment = rotationSpeed * (indexBias - rotationOffset);
                    increment = MathHelper.Clamp(increment, -0.4f, 0.4f);
                    if (Last) increment *= 1.4f;
                    rotation += increment;
                }


                Velocity += (Follow - Position) / (Last ? 30f : 50f) - Velocity / 10f;

                LUtils.Particles.SetSpawningModules(new SpawnRandomly(0.1f));
                float y = CurrentNodeCache.Last().Position.Y;

                if (rotationOffset > 0.8f)
                {
                    for (int i = 0; i < CurrentNodeCache.Count - 1; i++)
                    {
                        Vector2 v = (CurrentNodeCache[i].Position - new Vector2(0, y * 1.5f)).RotatedBy(rotation);

                        LUtils.Particles.SpawnParticles(
                        Position + v + Main.screenPosition,
                        Vector2.One.RotatedBy(Main.rand.NextFloat(-3f, 3f)) * 0.1f, 3,
                        Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(1)),
                        new SlowDown(0.97f),
                        new SetMask(Asset.GetTexture("Masks/RadialGradient")), new AfterImageTrail(1f),
                        new SetLighting(Color.Yellow.ToVector3(), 0.1f),
                        new RotateVelocity(Main.rand.NextFloat(-0.012f, 0.012f)), new SetShrinkSize(0.87f), new SetTimeLeft(15), new ZigzagMotion(5f, 3f));
                    }
                }

                if (rotationOffset > 0.99f)
                {
                    player.GetModPlayer<LinuxPlayer>().ScreenShake = 20;

                }

                if (SlashCount >= MaxSlashLength && rotationOffset > 0.98f)
                {
                    player.GetModPlayer<LinuxPlayer>().ScreenShake = 40;
                }

                if (rotationOffset < 0.5f && SlashCount >= MaxSlashLength)
                {
                    Deactivate();
                }
            }
            if (CurrentNodeCache.Count > 0)
            {
                DrawInsignia();
                DrawMasks();
            }

            Position += Velocity;
            DeltaMouseCache = Main.MouseWorld;
            player.GetModPlayer<LinuxPlayer>().SetScreenPosition(Main.MouseWorld);
        }
        List<int> projectiles = new List<int>();
        protected override void OnActivate(Player player)
        {
            CurrentNodeCache = Maker.CurrentNodeCache;
            float Y = CurrentNodeCache.Last().Position.Y;
            Position = Maker.CurrentMousePos + new Vector2(0, Y*1.5f);
            Follow = Maker.CurrentMousePos + new Vector2(0, Y * 1.5f);
            Width = 1;
            down = 0;
            rotationSpeed = 0;
            rotationOffset = 0;
            rotation = 0;
            AlphaAddon = 0;
            rotation = 0;
            Velocity = Vector2.Zero;
            projectiles = new List<int>();
            SlashCount = -1;
            for (int i = 0; i < CurrentNodeCache.Count; i++)
            {
                projectiles.Add(Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<RapierProjectile>(), 10, 1f, Main.myPlayer, i));
            }

            DirectionPath.Clear();
            positionCache.Clear();
            rotationCache.Clear();
            CurrentDirection.Clear();
        }
        protected override void UpdatePassive(Player player)
        {
            if (Width > 0)
            {
                Width += 0.1f + down;
                down -= 0.01f;

                DrawParticleEffects();
            }
            else
            {
                Width = 0;
            }

            Position += Velocity;
            Velocity += (Follow - Position) / 50f - Velocity / 10f;

            DrawInsignia();
        }

        public void DrawInsignia()
        {
            if (CurrentNodeCache.Count > 0)
            {
                float Y = CurrentNodeCache.Last().Position.Y;

                ScreenMapPass.Instance.GetMap("InsigniaMap").DrawToBatchedTarget((SpriteBatch sb) =>
                {
                    for (int i = 0; i < CurrentNodeCache.Count - 1; i++)
                    {
                        Vector2 p = Position;
                        float distance = Vector2.Distance(CurrentNodeCache[i + 1].Position, CurrentNodeCache[i].Position);
                        for (float a = 0; a <= 1; a += 1 / distance)
                        {
                            Vector2 v = (Vector2.Lerp(CurrentNodeCache[i].Position, CurrentNodeCache[i + 1].Position, a) - new Vector2(0, Y * 1.5f)).RotatedBy(rotation);
                            LUtils.DrawCircle(p + v, new Vector2((i / (float)CurrentNodeCache.Count) * 10 + 8f) * Width, Color.Lerp(Color.Black, Color.White, i / (float)CurrentNodeCache.Count));
                        }
                    }
                    bool Last = SlashCount >= MaxSlashLength;

                    for (int k = 0; k < positionCache.Count - 1; k++)
                    {
                        float distanceImage = Vector2.Distance(positionCache[k + 1], positionCache[k]);
                        float width = k / (float)rotationCache.Count * rotationOffset;
                        float rotation = rotationCache[k];
                        Vector2 p = positionCache[k];
                        for (int i = 0; i < CurrentNodeCache.Count - 1; i++)
                        {
                            float distance = Vector2.Distance(CurrentNodeCache[i + 1].Position, CurrentNodeCache[i].Position);

                            for (float a = 0; a <= 1; a += 1f / distance)
                            {
                                Vector2 v = (Vector2.Lerp(CurrentNodeCache[i].Position, CurrentNodeCache[i + 1].Position, a) - new Vector2(0, Y * 1.5f)).RotatedBy(rotation);
                                LUtils.DrawCircle(p + v, new Vector2((i / (float)CurrentNodeCache.Count) * 10 + 8f) * Width * (width * width * width), Color.Lerp(Color.Black, Color.White, i / (float)CurrentNodeCache.Count));
                            }
                        }

                    }
                });
            }
        }

        protected override void OnDeactivate(Player player)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                Main.projectile[projectiles[i]].Kill();
            }
        }
        public void DrawParticleEffects()
        {
            LUtils.Particles.SetSpawningModules(new SpawnRandomly(0.1f));
            float Y = CurrentNodeCache.Last().Position.Y;

            for (int i = 0; i < CurrentNodeCache.Count - 1; i++)
            {
                Vector2 v = (CurrentNodeCache[i].Position - new Vector2(0, Y * 1.5f)).RotatedBy(rotation);

                LUtils.Particles.SpawnParticles(
                Position + v + Main.screenPosition,
                Vector2.One.RotatedBy(Main.rand.NextFloat(-3f, 3f)) * 0.1f, 3,
                Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(1)),
                new SlowDown(0.97f),
                new SetMask(Asset.GetTexture("Masks/RadialGradient")), new AfterImageTrail(1f),
                new SetLighting(Color.Yellow.ToVector3(), 0.1f),
                new RotateVelocity(Main.rand.NextFloat(-0.012f, 0.012f)), new SetShrinkSize(0.87f), new SetTimeLeft(15), new ZigzagMotion(5f, 3f));
            }
        }
        public void DrawMasks()
        {
            float Y = CurrentNodeCache.Last().Position.Y;

            AdditiveCalls.Instance.AddCall((SpriteBatch sb) =>
            {
                for (int i = 0; i < CurrentNodeCache.Count - 1; i++)
                {
                    Vector2 p = Position;
                    float distance = Vector2.Distance(CurrentNodeCache[i + 1].Position, CurrentNodeCache[i].Position);
                    for (float a = 0; a <= 1; a += 1 / distance)
                    {
                        Texture2D tex = LUtils.RadialMask;
                        Vector2 v = (Vector2.Lerp(CurrentNodeCache[i].Position, CurrentNodeCache[i + 1].Position, a) - new Vector2(0, Y * 1.5f)).RotatedBy(rotation);
                        float velCap = MathHelper.Clamp(Velocity.Length(), 0, 4);
                        sb.Draw(tex, p + v, tex.Bounds, Color.Yellow * 0.1f * Math.Min(1, Width),
                            0f, tex.TextureCenter(),
                            (0.05f + (i / (float)CurrentNodeCache.Count) * 0.1f) * Math.Min(1, Width) + Math.Abs(velCap * 0.09f) + AlphaAddon,
                            SpriteEffects.None, 0f);
                    }
                }
            });
        }
    }
}


