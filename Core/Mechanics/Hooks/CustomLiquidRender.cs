using LinuxMod.Core.Helper.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class LiquidRender : Mechanic
    {
        public static LiquidRender Instance;
        public LiquidHost liquidHost = new LiquidHost();

        public override void AddHooks()
        {
            On.Terraria.Main.UpdateDisplaySettings += Main_UpdateDisplaySettings;
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            Instance = this;
        }

        private void Main_UpdateDisplaySettings(On.Terraria.Main.orig_UpdateDisplaySettings orig, Main self)
        {
            ScreenMapPass.Instance?.Maps?.OrderedRenderPassBatched(Main.spriteBatch, Main.graphics.GraphicsDevice);

            orig(self);
        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            ScreenMapPass.Instance.Maps.OrderedShaderPass();

            liquidHost.UpdateLiquids();

            orig(self);

        }
    }
}