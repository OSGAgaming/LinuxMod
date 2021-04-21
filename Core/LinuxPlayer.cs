
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
using LinuxMod.Core.Subworlds;
using LinuxMod;
using LinuxMod.Core.Subworlds.LinuxSubworlds;

namespace LinuxMod.Core
{
    public class LinuxPlayer : ModPlayer
    {
        public override void OnEnterWorld(Player player)
        {
            if (!Main.LocalPlayer.GetModPlayer<SubworldPlayer>().InSubworld)
                SubworldManager.EnterSubworld<CutsceneSubworld>();
        }
    }
}
