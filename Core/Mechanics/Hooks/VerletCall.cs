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
            LinuxMod.verletSystem.Update();
            LinuxMod.verletSystem.Draw(Main.spriteBatch);

            orig(self);
        }
    }
}