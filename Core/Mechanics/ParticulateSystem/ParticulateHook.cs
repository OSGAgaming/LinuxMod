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
    public class ParticulateHook : Mechanic
    {
        ParticulateHost PH;
        private int MaxMistFields => 1;

        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            Main.OnPreDraw += Main_OnPreDraw;
            PH = new ParticulateHost();
        }

        public override void Unload()
        {
            On.Terraria.Main.DrawWoF -= Main_DrawWoF;
            Main.OnPreDraw -= Main_OnPreDraw;

            PH = null;
        }
        private void Main_OnPreDraw(GameTime obj)
        {
            PH?.Update();
        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            orig(self);
            PH?.Draw(Main.spriteBatch);

            if (LinuxInput.JustClicked && PH.Particulates.Count < MaxMistFields)
            {
                PH?.GenerateMistField(Main.MouseWorld, 400, 600);
            }

            if (Main.LocalPlayer.controlMount)
            {
                PH.Particulates.Clear();
            }
        }
    }
}


