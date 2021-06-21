using LinuxMod.Core.Mechanics.Primitives;
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
    public class Targets : Mechanic
    {
        public static Targets Instance;

        public RenderTarget2D ScaledTileTarget { get; set; }

        public override void OnLoad()
        {
            ScaledTileTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
        }
        public override void AddHooks()
        {
            Main.OnPreDraw += Main_OnPreDraw;
            Instance = this;
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