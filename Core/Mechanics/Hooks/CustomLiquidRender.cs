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
            On.Terraria.Main.DrawCachedNPCs += Main_DrawCachedNPCs;
            On.Terraria.Main.SortDrawCacheWorms += Main_SortDrawCacheWorms;
            Instance = this;
        }

        private void Main_SortDrawCacheWorms(On.Terraria.Main.orig_SortDrawCacheWorms orig, Main self)
        {
            orig(self);
        }
        private void Main_DrawCachedNPCs(On.Terraria.Main.orig_DrawCachedNPCs orig, Main self, System.Collections.Generic.List<int> npcCache, bool behindTiles)
        {
            orig(self, npcCache, behindTiles);
        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            orig(self);
            liquidHost.UpdateLiquids();
            ScreenMapPass.Instance.Maps.OrderedRenderPass(Main.spriteBatch, Main.graphics.GraphicsDevice);
        }
    }
}