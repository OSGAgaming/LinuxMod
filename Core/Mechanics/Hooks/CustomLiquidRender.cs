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
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            On.Terraria.Main.UpdateDisplaySettings += Main_UpdateDisplaySettings1;
            Instance = this;
        }

        private void Main_UpdateDisplaySettings1(On.Terraria.Main.orig_UpdateDisplaySettings orig, Main self)
        {

            orig(self);
        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            liquidHost.UpdateLiquids();

            orig(self);

        }
    }
}