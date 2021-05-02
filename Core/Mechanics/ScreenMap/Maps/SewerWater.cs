
using LinuxMod.Core.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;

namespace LinuxMod.Core.Mechanics.ScreenMap
{
    public class SewerWater : MapPass
    {
        protected override Effect MapEffect => LinuxMod.WaterWallReflection;

        internal override void OnApplyShader()
        {
            Utils.ActivateScreenShader("Linux:WaterWallReflection");
            Utils.GetScreenShader("Linux:WaterWallReflection").Shader.Parameters["noiseTexture"]?.SetValue(Asset.GetTexture("Noise/noise"));
            Utils.GetScreenShader("Linux:WaterWallReflection").Shader.Parameters["waterReflection"].SetValue(MapTarget);
            Utils.GetScreenShader("Linux:WaterWallReflection").UseIntensity(Main.GameUpdateCount);
        }

        public override void Load()
        {
            MapTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, 2560, 1440);
        }
    }
    //Too lazy to inherit. Just want it to fucking work
}