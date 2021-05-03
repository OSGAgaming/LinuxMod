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
            On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
            Instance = this;
        }

        private void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            Primitives.Draw(Main.spriteBatch);
            orig(self);
        }

        public void CreateTrail(Primitive PT) => Primitives._trails.Add(PT);

    }
}