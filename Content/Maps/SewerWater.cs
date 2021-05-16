
using LinuxMod.Core.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace LinuxMod.Core.Mechanics.ScreenMap
{
    public class SewerWater : MapPass
    {
        protected override string MapEffectName => "Linux:Sewers";
        public override int Priority => 0;

    }
    //Too lazy to inherit. Just want it to fucking work
}