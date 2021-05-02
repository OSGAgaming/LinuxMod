
using LinuxMod.Core.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;

namespace LinuxMod.Core.Mechanics.ScreenMap
{
    public delegate void MapRender(SpriteBatch spriteBatch);
    public abstract class MapPass
    {
        public int Index;

        public RenderTarget2D MapTarget;

        internal event MapRender MapActions;
        protected abstract Effect MapEffect { get; }
        internal virtual void OnApplyShader()
        {
            MapEffect?.CurrentTechnique.Passes[0].Apply();
        }
        public void ApplyShader()
        {
            MapEffect?.Parameters["noiseTexture"]?.SetValue(Asset.GetTexture("Noise/noise"));
            OnApplyShader();
        }

        public void DrawToTarget(MapRender method) => MapActions += method;
        public void Render(SpriteBatch spriteBatch, GraphicsDevice GD)
        {
            GD.SetRenderTarget(MapTarget);
            GD.Clear(Color.Transparent);
            MapActions?.Invoke(spriteBatch);
            OnDraw();
            MapActions = null;
        }

        internal virtual void OnDraw() { }

        public Map Parent;
        public MapPass()
        {
            Load();
        }

        public virtual void Load()
        {
            MapTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, 2560, 1440);
        }
    }
}