using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core.Mechanics
{
    public class ModelHost : Mechanic
    {
        internal List<ModelComponent> modelComponents = new List<ModelComponent>();
        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
        }
        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            foreach(ModelComponent mc in modelComponents)
            {
                mc.Draw(Main.spriteBatch);
            }
            orig(self);
        }
    }
}