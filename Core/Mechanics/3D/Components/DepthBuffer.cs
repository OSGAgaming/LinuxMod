using LinuxMod.Core.Mechanics.Primitives;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace LinuxMod.Core.Mechanics
{
    public static class DepthBuffer
    {
        private static Dictionary<string, DepthLayer> layers;

        private static void Order() => layers.OrderBy(n => -n.Value.Priority);

        public static void Load() => layers = new Dictionary<string, DepthLayer>();

        public static void Unload()
        {
            layers.Clear();
            layers = null;
        }

        public static void RegisterLayer(DepthLayer layer, string name)
        {
            if (layers.ContainsKey(name)) return;

            layers.Add(name, layer);
            Order();
        }

        public static void DrawLayers(SpriteBatch sb)
        {
            foreach (DepthLayer layer in layers.Values)
                layer.Draw(sb);
        }

        public static void ClearCalls()
        {
            foreach (DepthLayer layer in layers.Values)
                layer.ClearCalls();
        }

        public static void DrawLayersToTarget(LintitySet scene, SpriteBatch sb)
        {
            foreach (ILayeredDraw entity in scene.Drawables)
            {
                if (layers.ContainsKey(entity.Layer) || entity.Layer == null)
                {
                    if (entity is IndexedPrimitive)
                        layers[entity.Layer ?? "Default"].AppendPrimitiveCall(entity.Draw);
                    else
                        layers[entity.Layer ?? "Default"].AppendCall(entity.Draw);
                }
            }


            RenderTargetBinding[] oldtargets1 = LocalRenderer.Device.GetRenderTargets();

            foreach (DepthLayer layer in layers.Values)
                layer.DrawToTarget(sb);

            LocalRenderer.Device.SetRenderTargets(oldtargets1);
        }

        public static DepthLayer GetLayer(string layerName)
        {
            if (layerName != null)
                if (layers.ContainsKey(layerName))
                    return layers[layerName];

            return layers["Default"];
        }
    }

    public class DepthLayer
    {
        private event Action<SpriteBatch> DrawCalls;
        private event Action<SpriteBatch> PrimitiveCalls;

        public RenderTarget2D Target { get; set; }

        public Effect LayerEffect { get; set; }

        public CameraTransform Camera { get; set; }

        public float Priority { get; set; }

        public Rectangle ScissorSource { get; set; }

        public Rectangle Destination { get; set; }

        public DepthLayer(float priority, CameraTransform camera, Effect effect = null, Rectangle scissor = default, Rectangle destination = default)
        {
            LayerEffect = effect;
            Priority = priority;
            Camera = camera;

            ScissorSource = scissor == default ? LocalRenderer.MaxResolutionBounds : scissor;
            Destination = destination == default ? LocalRenderer.MaxResolutionBounds : destination;

            Target = new RenderTarget2D(LocalRenderer.Device, LocalRenderer.MaxResolution.X, LocalRenderer.MaxResolution.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        }

        public virtual void OnDraw() { }

        public void AppendCall(Action<SpriteBatch> call) => DrawCalls += call;

        public void AppendPrimitiveCall(Action<SpriteBatch> call) => PrimitiveCalls += call;


        public void DrawAllCalls(SpriteBatch sb)
        {
            DrawCalls?.Invoke(sb);
            PrimitiveCalls?.Invoke(sb);
        }

        public void ClearCalls()
        {
            DrawCalls = null;
            PrimitiveCalls = null;
        }

        public void DrawToTarget(SpriteBatch sb)
        {
            Camera.Update();

            LocalRenderer.Device.SetRenderTarget(Target);
            LocalRenderer.Device.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            LayerEffect?.CurrentTechnique.Passes[0]?.Apply();
            DrawCalls?.Invoke(sb);
            sb.End();

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            PrimitiveCalls?.Invoke(sb);
            sb.End();

        }

        public void Draw(SpriteBatch sb)
        {
            OnDraw();

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            LayerEffect?.CurrentTechnique.Passes[0]?.Apply();
            sb.Draw(Target, Destination, ScissorSource, Color.White);

            sb.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
