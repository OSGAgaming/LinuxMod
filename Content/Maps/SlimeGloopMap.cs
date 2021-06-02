
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
    public class SGMap : MapPass
    {
        protected override string MapEffectName => "Linux:SlimeGloop";
        public override int Priority => 5;

    }

    public class SGMapPass2 : MapPass
    {
        protected override string MapEffectName => "Linux:SlimeGloop2";
        public override RenderTarget2D ManualTarget => ScreenMapPass.Instance.GetMap("SGMap").MapTarget;
        public override int Priority => 6;
    }
    //Too lazy to inherit. Just want it to fucking work
}