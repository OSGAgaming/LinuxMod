using LinuxMod.Core.Mechanics.Verlet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core.Mechanics
{
    public class VerletCall : Mechanic
    {
        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
        }
        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            LinuxMod.GetLoadable<VerletSystem>().Update();
            LinuxMod.GetLoadable<VerletSystem>().Draw(Main.spriteBatch);

            orig(self);
        }
    }
}