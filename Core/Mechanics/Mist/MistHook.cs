using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class MistHook : Mechanic
    {
        MistFieldHost MFH;
        private int MaxMistFields => 2;

        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            MFH = new MistFieldHost();
        }
        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            orig(self);

            if (LinuxInput.JustClicked && MFH.MistFields.Count < MaxMistFields)
            {
                MFH?.GenerateMistField(32, 16, Main.MouseWorld);
            }

            MFH.Update();
            MFH.Draw(Main.spriteBatch);
        }
    }
}


