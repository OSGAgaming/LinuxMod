using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class PrimitivePass : Mechanic
    {
        public static PrimitivePass Instance;
        public PrimitiveManager Primitives = new PrimitiveManager();
        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            Instance = this;
        }

        public void CreateTrail(Primitive PT) => Primitives._trails.Add(PT);

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            Primitives.Draw(Main.spriteBatch);
            orig(self);
        }
    }
}