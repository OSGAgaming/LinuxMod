
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
    public class WaterWall : MapPass
    {
        protected override string MapEffectName => "Linux:WaterWall";
        public override int Priority => 1;

        internal override void OnApplyShader()
        {
            MapEffect.Shader.Parameters["Water"].SetValue(Asset.GetTexture("Noise/WaterShaderLightMap"));
        }

    }
    //Too lazy to inherit. Just want it to fucking work
}