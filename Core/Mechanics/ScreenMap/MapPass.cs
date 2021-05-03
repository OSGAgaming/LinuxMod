
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
    public delegate void MapRender(SpriteBatch spriteBatch);
    public abstract class MapPass
    {
        public int Index;

        public RenderTarget2D MapTarget;

        internal event MapRender MapActions;

        public RenderTarget2D PixelationTarget { get; set; }

        public int PixelationFactor { get; } = 1;

        protected abstract string MapEffectName { get; }

        protected ScreenShaderData MapEffect => LUtils.GetScreenShader(MapEffectName);
        internal virtual void OnApplyShader() { }
        public virtual void Load()
        {
            MapTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            PixelationTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / PixelationFactor, Main.screenHeight/ PixelationFactor);
        }
        public void ApplyShader()
        {
            MapEffect?.Shader.Parameters["Noise"]?.SetValue(Asset.GetTexture("Noise/noise"));
            MapEffect.Shader.Parameters["Map"].SetValue(MapTarget);

            MapEffect.UseIntensity(Main.GameUpdateCount);

            OnApplyShader();

            LUtils.ActivateScreenShader(MapEffectName);
        }

        public void DrawToTarget(MapRender method) => MapActions += method;
        public void Render(SpriteBatch spriteBatch, GraphicsDevice GD)
        {
            GD.SetRenderTarget(MapTarget);
            GD.Clear(Color.Transparent);

            spriteBatch.Draw(PixelationTarget, new Rectangle(0,0, Main.screenWidth, Main.screenHeight), Color.White);
        }

        public void DrawToPixelationTarget(SpriteBatch spriteBatch, GraphicsDevice GD)
        {
            GD.SetRenderTarget(PixelationTarget);
            GD.Clear(Color.Transparent);

            MapActions?.Invoke(spriteBatch);
            MapActions = null;
        }

        public Map Parent;
        public MapPass() => Load();

    }
}