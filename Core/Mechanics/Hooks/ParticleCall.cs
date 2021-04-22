using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class ParticleCall : Mechanic
    {
        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            orig(self);

            LinuxMod.GlobalZone.Update();
            LinuxMod.GlobalZone.Draw(Main.spriteBatch);
        }
    }
}