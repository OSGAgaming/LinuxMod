
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
    public class TileReflectionMap : MapPass
    {
        protected override string MapEffectName => "Linux:TileReflection";
        public override int Priority => 7;

        internal override void OnApplyShader()
        {
            MapEffect.Shader.Parameters["PlayerMap"].SetValue(ScreenMapPass.Instance.GetMap("TileReflectableMap").MapTarget);
        }

    }

    public class TileReflectableMap : MapPass
    {
        protected override string MapEffectName => "";
        public override int Priority => 6;

    }
}