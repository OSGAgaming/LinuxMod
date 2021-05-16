
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
        internal event MapRender BatchedCalls;

        internal event MapRender PrimitiveCalls;

        public RenderTarget2D MapTarget;

        public virtual RenderTarget2D ManualTarget => null;

        public abstract int Priority { get; }

        protected abstract string MapEffectName { get; }

        protected ScreenShaderData MapEffect => LUtils.GetScreenShader(MapEffectName);

        internal virtual void OnApplyShader() { }
        public virtual void Load()
        {
            MapTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
        }

        public void ApplyShader()
        {
            if (ManualTarget != null) MapTarget = ManualTarget;

            MapEffect?.Shader.Parameters["Noise"]?.SetValue(Asset.GetTexture("Noise/noise"));
            MapEffect.Shader.Parameters["Map"].SetValue(MapTarget);
            MapEffect.Shader.Parameters["TileTarget"]?.SetValue(Targets.Instance.ScaledTileTarget);
            MapEffect.Shader.Parameters["WallTarget"]?.SetValue(Main.instance.wallTarget);

            MapEffect.UseIntensity(Main.GameUpdateCount);

            OnApplyShader();

            LUtils.ActivateScreenShader(MapEffectName);
        }

        public void DrawToBatchedTarget(MapRender method) => BatchedCalls += method;

        public void DrawToPrimitiveTarget(MapRender method) => PrimitiveCalls += method;

        public void RenderBatched(SpriteBatch spriteBatch, GraphicsDevice GD)
        {
            BatchedCalls?.Invoke(spriteBatch);
            BatchedCalls = null;
        }

        public void RenderPrimitive(SpriteBatch spriteBatch, GraphicsDevice GD)
        {
            PrimitiveCalls?.Invoke(spriteBatch);
            PrimitiveCalls = null;
        }

        public Map Parent;
        public MapPass() => Load();

    }
}