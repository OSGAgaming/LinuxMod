
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
    public class AdditiveMap : MapPass
    {
        protected override string MapEffectName => "Linux:Additive";
        public override int Priority => 5;
    }
    //Too lazy to inherit. Just want it to fucking work
}