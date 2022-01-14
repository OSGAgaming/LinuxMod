using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class Targets : Mechanic
    {
        public static Targets Instance;
        public RenderTarget2D playerDrawData;

        public RenderTarget2D ScaledTileTarget { get; set; }

        public override void OnLoad()
        {
            ScaledTileTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

            if (!Main.dedServ)
            {
                playerDrawData = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            }
        }
        public override void AddHooks()
        {
            Main.OnPreDraw += Main_OnPreDraw;
            On.Terraria.Projectile.Update += Projectile_Update;

            Instance = this;
        }

        private void Projectile_Update(On.Terraria.Projectile.orig_Update orig, Projectile self, int i)
        {
            ScreenMapPass.Instance.GetMap("TileReflectableMap").DrawToBatchedTarget((spriteBatch) =>
            {
                Texture2D tex = Main.projectileTexture[self.type];
                if(tex != null && self.active) spriteBatch.Draw(tex, self.position.ForDraw() + new Vector2(30, -30), tex.Bounds, Color.White, self.rotation, Vector2.Zero, self.scale, SpriteEffects.None, 0f);
            });

            orig(self, i);
        }

        private void Main_OnPreDraw(GameTime obj)
        {
            /*if(LinuxInput.JustClicked)
            {
                PhysicsObject Object = new PhysicsObject();

                int X = (int)Main.MouseWorld.X;
                int Y = (int)Main.MouseWorld.Y;

                Object.LoadModule(new PolygonModule(new Rectangle(X,Y, 20, 20)));
                Object.LoadModule(new TileCollisionModule());
                Object.LoadModule(new PhysicsCollisionModule());
                Object.LoadModule(new VerletModule());
                Object.LoadModule(new RigidBodyModule(0.004f));

                Object.Center = Main.MouseWorld;
            }
            if (Main.LocalPlayer.controlUp)
            {
                GetMechanic<PhysicsObjectHook>().Objects.Objects.Clear();
                LinuxMod.verletSystem.Sticks.Clear();
                LinuxMod.verletSystem.Points.Clear();

            }
            */

            RenderTargetBinding[] oldtargets2 = Main.graphics.GraphicsDevice.GetRenderTargets();
            Main.graphics.GraphicsDevice.SetRenderTarget(playerDrawData);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin();

            for (int i = 0; i <= Main.playerDrawData.Count; i++)
            {
                int num = -1;
                if (num != 0)
                {
                    Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                    num = 0;
                }

                if (i != Main.playerDrawData.Count)
                {
                    DrawData value = Main.playerDrawData[i];
                    if (value.shader >= 0)
                    {
                        GameShaders.Hair.Apply(0, Main.LocalPlayer, value);
                        GameShaders.Armor.Apply(value.shader, Main.LocalPlayer, value);
                    }
                    else if (Main.LocalPlayer.head == 0)
                    {
                        GameShaders.Hair.Apply(0, Main.LocalPlayer, value);
                        GameShaders.Armor.Apply(Main.LocalPlayer.cHead, Main.LocalPlayer, value);
                    }
                    else
                    {
                        GameShaders.Armor.Apply(0, Main.LocalPlayer, value);
                        GameShaders.Hair.Apply((short)(-value.shader), Main.LocalPlayer, value);
                    }
                    if (!value.sourceRect.HasValue)
                    {
                        value.sourceRect = value.texture.Frame();
                    }
                    num = value.shader;
                    if (value.texture != null)
                    {
                        Main.spriteBatch.Draw(value.texture, value.position, value.sourceRect, value.color, value.rotation, value.origin, value.scale, value.effect, 0f);
                    }
                }
            }

            Main.spriteBatch.End();
            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets2);

            ScreenMapPass.Instance.GetMap("TileReflectableMap").DrawToBatchedTarget((spriteBatch) =>
            {
                spriteBatch.Draw(playerDrawData, new Vector2(30, -30), Color.White);
            });

            ScreenMapPass.Instance.GetMap("TileReflectionMap").DrawToBatchedTarget((spriteBatch) =>
            {
                for (int i = -8; i < 8; i++)
                {
                    for (int j = -8; j < 8; j++)
                    {
                        Point p = (Main.LocalPlayer.position / 16).ToPoint();
                        Point pij = new Point(p.X + i, p.Y + j);

                        if (WorldGen.InWorld(pij.X, pij.Y))
                        {
                            Tile tile = Framing.GetTileSafely(pij);
                            ushort type = tile.wall;

                            if (type == WallID.Glass
                             || type == WallID.BlueStainedGlass
                             || type == WallID.GreenStainedGlass
                             || type == WallID.PurpleStainedGlass
                             || type == WallID.YellowStainedGlass
                             || type == WallID.RedStainedGlass)
                            {
                                Vector2 pos = pij.ToVector2() * 16;
                                Texture2D tex = Main.wallTexture[type];
                                if (tex != null) spriteBatch.Draw(Main.wallTexture[type], pos.ForDraw() - new Vector2(8, 8), new Rectangle(tile.wallFrameX(), tile.wallFrameY(), 36, 36), Color.White);
                            }
                        }
                    }
                }
            });

            RenderTargetBinding[] oldtargets1 = Main.graphics.GraphicsDevice.GetRenderTargets();

            Matrix matrix = Main.GameViewMatrix.ZoomMatrix;

            GraphicsDevice GD = Main.graphics.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

            GD.SetRenderTarget(ScaledTileTarget);
            GD.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, matrix);
            Main.spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Color.White);
            sb.End();

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets1);
        }
    }
}