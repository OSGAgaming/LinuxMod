
using LinuxMod.Core.Assets;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;

namespace LinuxMod.Core.Mechanics.Primitives
{
    public struct VertexPositionNormalTextureTangent : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector3 Tangent;

        public static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
              new VertexElement(VertexElementByteOffset.PositionStartOffset(), VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
              new VertexElement(VertexElementByteOffset.OffsetVector3(), VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
              new VertexElement(VertexElementByteOffset.OffsetVector2(), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
              new VertexElement(VertexElementByteOffset.OffsetVector3(), VertexElementFormat.Vector3, VertexElementUsage.Normal, 1)
        );
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    }
    /// <summary>
    /// This is a helper struct for tallying byte offsets
    /// </summary>
    public struct VertexElementByteOffset
    {
        public static int currentByteSize = 0;
        [STAThread]
        public static int PositionStartOffset() { currentByteSize = 0; var s = sizeof(float) * 3; currentByteSize += s; return currentByteSize - s; }
        public static int Offset(float n) { var s = sizeof(float); currentByteSize += s; return currentByteSize - s; }
        public static int Offset(Vector2 n) { var s = sizeof(float) * 2; currentByteSize += s; return currentByteSize - s; }
        public static int Offset(Color n) { var s = sizeof(int); currentByteSize += s; return currentByteSize - s; }
        public static int Offset(Vector3 n) { var s = sizeof(float) * 3; currentByteSize += s; return currentByteSize - s; }
        public static int Offset(Vector4 n) { var s = sizeof(float) * 4; currentByteSize += s; return currentByteSize - s; }

        public static int OffsetFloat() { var s = sizeof(float); currentByteSize += s; return currentByteSize - s; }
        public static int OffsetColor() { var s = sizeof(int); currentByteSize += s; return currentByteSize - s; }
        public static int OffsetVector2() { var s = sizeof(float) * 2; currentByteSize += s; return currentByteSize - s; }
        public static int OffsetVector3() { var s = sizeof(float) * 3; currentByteSize += s; return currentByteSize - s; }
        public static int OffsetVector4() { var s = sizeof(float) * 4; currentByteSize += s; return currentByteSize - s; }
    }

    public class SeamapWater : ILayeredDraw, ILoadable
    {
        public RenderTarget2D OcclusionMap;
        public RenderTarget2D ReverseOcclusionMap;

        public RenderTarget2D EntityMap;
        public RenderTarget2D ReverseEntityMap;

        public RenderTarget2D BelowWater;
        public RenderTarget2D AboveWater;

        public RenderTarget2D WaterTarget;
        public RenderTarget2D WaterOcclusion;

        public List<SeamapWaterMesh> Meshes;
        public Action<SpriteBatch, int> ReflectionCalls;
        public Action<SpriteBatch, int> SkyboxCall;

        public string Layer { get; set; } = "Seamap";

        public void AppendReflectionCall(Action<SpriteBatch, int> call) => ReflectionCalls += call;

        public void AppendSkyboxCall(Action<SpriteBatch, int> call) => SkyboxCall += call;

        public void Draw(SpriteBatch sb)
        {

        }

        public void DrawInverseWaterMeshes(SpriteBatch sb)
        {
            foreach (SeamapWaterMesh Mesh in Meshes)
            {
                Mesh.DrawWaterInverse(sb);
            }
        }

        public void DrawWaterMeshes(SpriteBatch sb, Color color)
        {
            foreach (SeamapWaterMesh Mesh in Meshes)
            {
                Mesh.DrawWater(sb, color);
            }
        }

        public void DrawWaterMeshesEffect(SpriteBatch sb, Effect effect)
        {
            foreach (SeamapWaterMesh Mesh in Meshes)
            {
                Mesh.param = (e) =>
                {
                    DepthLayer currentlayer = DepthBuffer.GetLayer(Mesh.Layer);

                    e.Parameters["WorldViewProjection"].SetValue(
                        currentlayer.Camera.ViewMatrix *
                        currentlayer.Camera.ProjectionMatrix *
                        currentlayer.Camera.WorldMatrix);

                    e.Parameters["dUdVMap"].SetValue(Asset.GetTexture(Asset.NoiseDirectory + "noise2"));
                };
                Mesh.DrawEffectWater(sb, effect);
            }
        }

        public void ClearCalls() => ReflectionCalls = null;

        public void DrawOcclusionMap(SpriteBatch sb)
        {
            Main.graphics.GraphicsDevice.SetRenderTarget(OcclusionMap);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            DrawInverseWaterMeshes(sb);

            ReflectionCalls?.Invoke(sb, 0);

            sb.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(ReverseOcclusionMap);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            DrawWaterMeshes(sb, Color.Black);

            DepthBuffer.GetLayer(Layer).DrawAllCalls(sb);

            sb.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(EntityMap);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            //DepthBuffer.GetLayer(Layer).DrawAllCalls(sb);
            ReflectionCalls?.Invoke(sb, 0);

            sb.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(ReverseEntityMap);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            DepthBuffer.GetLayer(Layer).DrawAllCalls(sb);

            sb.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(WaterTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            DrawWaterMeshesEffect(sb, LinuxMod.DUDVMap);

            sb.End();
        }

        public void RenderWaterOcclusion(SpriteBatch sb)
        {
            Main.graphics.GraphicsDevice.SetRenderTarget(WaterOcclusion);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            //DepthBuffer.GetLayer("Skybox").DrawAllCalls(sb);

            sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            LinuxMod.WaterOcclusionEffect.Parameters["occlusionTexture"].SetValue(ReverseOcclusionMap);
            LinuxMod.WaterOcclusionEffect.Parameters["inverse"].SetValue(0);
            LinuxMod.WaterOcclusionEffect.CurrentTechnique.Passes[0].Apply();

            sb.Draw(ReverseEntityMap, OcclusionMap.Bounds, OcclusionMap.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            sb.End();
        }

        public void DrawAboveWater(SpriteBatch sb)
        {
            Main.graphics.GraphicsDevice.SetRenderTarget(AboveWater);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            SkyboxCall?.Invoke(sb, 0);
            SkyboxCall = null;

            sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            LinuxMod.WaterOcclusionEffect.Parameters["occlusionTexture"].SetValue(OcclusionMap);
            LinuxMod.WaterOcclusionEffect.Parameters["inverse"].SetValue(1);
            LinuxMod.WaterOcclusionEffect.CurrentTechnique.Passes[0].Apply();

            sb.Draw(EntityMap, OcclusionMap.Bounds, OcclusionMap.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            sb.End();
        }

        public void DrawBelowWater(SpriteBatch sb)
        {
            Main.graphics.GraphicsDevice.SetRenderTarget(BelowWater);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            //SkyboxCall?.Invoke(sb, 0);
            //SkyboxCall = null;

            sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            LinuxMod.WaterOcclusionEffect.Parameters["occlusionTexture"].SetValue(ReverseOcclusionMap);
            LinuxMod.WaterOcclusionEffect.Parameters["inverse"].SetValue(1);
            LinuxMod.WaterOcclusionEffect.CurrentTechnique.Passes[0].Apply();

            sb.Draw(ReverseEntityMap, OcclusionMap.Bounds, OcclusionMap.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            sb.End();
        }

        public void DrawWater(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            LinuxMod.ReflectRefractEffect.Parameters["reflectionMap"].SetValue(AboveWater);
            LinuxMod.ReflectRefractEffect.Parameters["refractionMap"].SetValue(BelowWater);
            LinuxMod.ReflectRefractEffect.Parameters["oclMap"].SetValue(WaterOcclusion);
            LinuxMod.ReflectRefractEffect.Parameters["colorMod"].SetValue(new Vector4(1.3f, 1.3f, 0.6f, 1f));
            LinuxMod.ReflectRefractEffect.Parameters["reflectionCoefficient"].SetValue(0.4f);
            LinuxMod.ReflectRefractEffect.Parameters["refractionCoefficient"].SetValue(1);
            LinuxMod.ReflectRefractEffect.Parameters["distortionCoefficient"].SetValue(0.1f);

            LinuxMod.ReflectRefractEffect.CurrentTechnique.Passes[0].Apply();

            sb.Draw(WaterTarget, DepthBuffer.GetLayer("Seamap").Destination, DepthBuffer.GetLayer("Seamap").ScissorSource, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
        }

        public void DrawTargets(SpriteBatch sb)
        {
            //Test
            LinuxTechTips.DrawRectangle(new Rectangle(0, 100, 100, 100), Color.Red, 1);
            LinuxTechTips.DrawRectangle(new Rectangle(0, 200, 100, 100), Color.Red, 1);
            LinuxTechTips.DrawRectangle(new Rectangle(0, 300, 100, 100), Color.Red, 1);

            sb.Draw(BelowWater, new Rectangle(0, 100, 100, 100), Color.White);
            sb.Draw(AboveWater, new Rectangle(0, 200, 100, 100), Color.White);
            sb.Draw(WaterTarget, new Rectangle(0, 300, 100, 100), Color.White);

            DrawWater(sb);
        }

        public void Load()
        {
            OcclusionMap = new RenderTarget2D(Main.graphics.GraphicsDevice, LocalRenderer.MaxResolution.X, LocalRenderer.MaxResolution.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            WaterOcclusion = new RenderTarget2D(Main.graphics.GraphicsDevice, LocalRenderer.MaxResolution.X, LocalRenderer.MaxResolution.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            AboveWater = new RenderTarget2D(Main.graphics.GraphicsDevice, LocalRenderer.MaxResolution.X, LocalRenderer.MaxResolution.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            ReverseOcclusionMap = new RenderTarget2D(Main.graphics.GraphicsDevice, LocalRenderer.MaxResolution.X, LocalRenderer.MaxResolution.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            EntityMap = new RenderTarget2D(Main.graphics.GraphicsDevice, LocalRenderer.MaxResolution.X, LocalRenderer.MaxResolution.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            ReverseEntityMap = new RenderTarget2D(Main.graphics.GraphicsDevice, LocalRenderer.MaxResolution.X, LocalRenderer.MaxResolution.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            WaterTarget = new RenderTarget2D(Main.graphics.GraphicsDevice,
                LocalRenderer.MaxResolution.X, LocalRenderer.MaxResolution.Y, false,
                LocalRenderer.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            BelowWater = new RenderTarget2D(Main.graphics.GraphicsDevice, LocalRenderer.MaxResolution.X, LocalRenderer.MaxResolution.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            Meshes = new List<SeamapWaterMesh>();
        }

        public void Unload()
        {

        }
    }

    public class SeamapWaterMesh : QuadMesh
    {
        public Color InternalColor;

        public SeamapWaterMesh(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color = default, string layer = "Default", Texture2D texture = null, Effect effect = null) :
            base(v1, v2, v3, v4, Color.Black, layer, texture, effect)
        {
            InternalColor = color;
            LinuxMod.GetLoadable<SeamapWater>().Meshes.Add(this);
        }

        public void DrawWaterInverse(SpriteBatch sb)
        {
            DepthLayer currentlayer = DepthBuffer.GetLayer(layer);

            basicEffect.VertexColorEnabled = true;

            Vector3 cameraPos = currentlayer.Camera.Transform.Position;

            Vector3 ViewTransform = Vector3.Multiply(cameraPos, new Vector3(1, -1, 1));
            Vector3 ViewTarget = ViewTransform + Vector3.Multiply(currentlayer.Camera.Direction, new Vector3(1, -1, 1));

            Matrix view = Matrix.CreateLookAt(ViewTransform, ViewTarget, Vector3.Up);

            basicEffect.View = view;
            basicEffect.Projection = currentlayer.Camera.ProjectionMatrix;
            basicEffect.World = currentlayer.Camera.WorldMatrix;

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Color = Color.Black;

            vertexBuffer.SetData(vertices);

            indexBuffer.SetData(indices);

            sb.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            sb.GraphicsDevice.Indices = indexBuffer;


            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                sb.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexPointer, 0, indexPointer / 3);
            }
        }

        public void DrawWater(SpriteBatch sb, Color color)
        {
            DepthLayer currentlayer = DepthBuffer.GetLayer(layer);

            basicEffect.VertexColorEnabled = true;

            basicEffect.View = currentlayer.Camera.ViewMatrix;
            basicEffect.Projection = currentlayer.Camera.ProjectionMatrix;
            basicEffect.World = currentlayer.Camera.WorldMatrix;

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Color = color;

            vertexBuffer.SetData(vertices);
            indexBuffer.SetData(indices);

            sb.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            sb.GraphicsDevice.Indices = indexBuffer;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                sb.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexPointer, 0, indexPointer / 3);
            }
        }

        public void DrawEffectWater(SpriteBatch sb, Effect effect)
        {
            DepthLayer currentlayer = DepthBuffer.GetLayer(layer);

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Color = Color.Blue;
            //param?.Invoke(effect);
            LinuxMod.DUDVMap.Parameters["WorldViewProjection"].SetValue(currentlayer.Camera.ViewMatrix * currentlayer.Camera.ProjectionMatrix);
            LinuxMod.DUDVMap.Parameters["dUdVMap"].SetValue(Asset.GetTexture(Asset.NoiseDirectory + "dUdVTexture"));
            LinuxMod.DUDVMap.Parameters["progress"].SetValue(Main.GameUpdateCount / 1000f);
            LinuxMod.DUDVMap.Parameters["coordDensity"].SetValue(new Vector2(3f, 3f));
            LinuxMod.DUDVMap.Parameters["Target"].SetValue(currentlayer.Camera.Transform.Position);
            LinuxMod.DUDVMap.Parameters["NormalMap"].SetValue(Asset.GetTexture(Asset.NoiseDirectory + "WaterNormalMap"));
            LinuxMod.DUDVMap.Parameters["World"].SetValue(Matrix.Identity);
            LinuxMod.DUDVMap.Parameters["SpecularPower"].SetValue(2);

            LinuxMod.DUDVMap.CurrentTechnique.Passes["Reflect"].Apply();

            vertexBuffer.SetData(vertices);
            indexBuffer.SetData(indices);

            sb.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            sb.GraphicsDevice.Indices = indexBuffer;

            sb.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            sb.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            sb.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexPointer, 0, indexPointer / 3);
        }
    }
}

