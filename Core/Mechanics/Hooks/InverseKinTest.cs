using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core.Mechanics
{
    public class IKTest : Mechanic
    {
        CorePart c;
        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
        }
        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                c = new CorePart(Main.MouseWorld, 2f);
            }

            if (c != null)
            {
                c.UpdateParts();
                c.DrawParts(Main.spriteBatch);
            }
            orig(self);
        }
    }
}