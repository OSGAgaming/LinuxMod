using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Primitives;
using LinuxMod.Core.Subworlds.LinuxSubworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class DepthSetCalls : Mechanic
    {
        public static DepthSetCalls Instance;
        public static Action<SpriteBatch> OnPreDraw;
        public static Action OnPreUpdate;
        public LintitySet CurrentLintitySet;

        public override void AddHooks()
        {
            Instance = this;
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            Main.OnPreDraw += Main_OnPreDraw;
        }

        public override void Unload()
        {
            On.Terraria.Main.DrawWoF -= Main_DrawWoF;
            Main.OnPreDraw -= Main_OnPreDraw;

            CurrentLintitySet = null;
            Instance = null;
            OnPreDraw = null;
        }
        private void Main_OnPreDraw(GameTime obj)
        {
            RenderTargetBinding[] oldtargets = Main.graphics.GraphicsDevice.GetRenderTargets();

            if (Main.gameMenu)
                return;

            if (CurrentLintitySet != null)
                DepthBuffer.DrawLayersToTarget(CurrentLintitySet, Main.spriteBatch);

            OnPreDraw?.Invoke(Main.spriteBatch);
            OnPreUpdate?.Invoke();

            DepthBuffer.ClearCalls();

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets);

        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            CurrentLintitySet?.Update();

            DepthBuffer.DrawLayers(Main.spriteBatch);
            //SeamapComponents.WaterContainer.Draw(Main.spriteBatch);
            orig(self);
        }
    }
}