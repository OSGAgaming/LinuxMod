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
        private int MaxMistFields => 1;

        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            Main.OnPreDraw += Main_OnPreDraw;
            MFH = new MistFieldHost();
        }

        public override void Unload()
        {
            On.Terraria.Main.DrawWoF -= Main_DrawWoF;
            Main.OnPreDraw -= Main_OnPreDraw;

            MFH = null;
        }
        private void Main_OnPreDraw(GameTime obj)
        {
            MFH?.Update();
        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            orig(self);
            MFH?.Draw(Main.spriteBatch);

            if (LinuxInput.JustClicked && MFH.MistFields.Count < MaxMistFields)
            {
                //MFH?.GenerateMistField(100, 1, Main.MouseWorld);
            }

        }
    }
}


