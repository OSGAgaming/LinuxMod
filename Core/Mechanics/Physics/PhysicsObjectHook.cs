using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class PhysicsObjectHook : Mechanic
    {
        public PhysicsObjectHost Objects = new PhysicsObjectHost();

        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
        }
        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            Objects.Update();
            Objects.Draw(Main.spriteBatch);

            orig(self);
        }
    }
}