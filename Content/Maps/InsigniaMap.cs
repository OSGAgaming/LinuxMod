
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
    public class InsigniaMap : MapPass
    {
        protected override string MapEffectName => "Linux:Insignia";
        public override int Priority => 3;

    }

    public class InsigniaMapPass2 : MapPass
    {
        protected override string MapEffectName => "Linux:Insignia2";
        public override RenderTarget2D ManualTarget => ScreenMapPass.Instance.GetMap("InsigniaMap").MapTarget;
        public override int Priority => 4;
    }
    //Too lazy to inherit. Just want it to fucking work
}