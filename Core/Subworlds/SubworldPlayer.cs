
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;

namespace LinuxMod.Core.Subworlds
{
    public class SubworldPlayer : ModPlayer
    {
        internal string PrimaryWorldName = "";

        public bool InSubworld;

        public Subworld CurrentSubworld = null;
        public override void PostUpdate()
        {
            if (!InSubworld) PrimaryWorldName = Main.ActiveWorldFileData.Name;

            CurrentSubworld?.PlayerUpdate(player);
        }
    }
}
