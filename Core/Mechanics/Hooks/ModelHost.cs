using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core.Mechanics
{
    public class ModelHost : Mechanic
    {
        internal List<ModelComponent> modelComponents;

        internal static event Action<SpriteBatch> DrawCalls;

        public RenderTarget2D ModelTarget { get; set; }

        public override void OnLoad()
        {
            modelComponents = new List<ModelComponent>();
            ModelTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth/2, Main.screenHeight/2);
        }

        public override void AddHooks()
        {
            Main.OnPreDraw += Main_OnPreDraw;
            On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
        }

        public override void Unload()
        {
            modelComponents = null;

            Main.OnPreDraw -= Main_OnPreDraw;
            On.Terraria.Main.DrawProjectiles -= Main_DrawProjectiles;
        }
        public static void SubscribeCall(Action<SpriteBatch> del) => DrawCalls += del;

        private void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            LinuxMod.ColourLimit.CurrentTechnique.Passes[0].Apply();
            if (ModelTarget != null) Main.spriteBatch.Draw(ModelTarget, new Rectangle(0,0, Main.screenWidth, Main.screenHeight), Color.White);

            Main.spriteBatch.End();
            orig(self);
        }
        private void Main_OnPreDraw(GameTime obj)
        {
            RenderTargetBinding[] oldtargets1 = Main.graphics.GraphicsDevice.GetRenderTargets();
            Matrix matrix = Main.GameViewMatrix.ZoomMatrix;

            GraphicsDevice GD = Main.graphics.GraphicsDevice;

            GD.SetRenderTarget(ModelTarget);
            GD.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            DrawCalls?.Invoke(Main.spriteBatch);
            DrawCalls = null;

            Main.spriteBatch.End();
            GD.SetRenderTargets(oldtargets1);
        }
    }
}