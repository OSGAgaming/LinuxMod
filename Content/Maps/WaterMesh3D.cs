
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

    public class WaterMeshContainer3D : ILayeredDraw, ILoad
    {
        public RenderTarget2D OcclusionMap;
        public RenderTarget2D ReverseOcclusionMap;

        public RenderTarget2D EntityMap;
        public RenderTarget2D ReverseEntityMap;

        public RenderTarget2D BelowWater;
        public RenderTarget2D AboveWater;

        public RenderTarget2D WaterTarget;
        public RenderTarget2D WaterOcclusion;

        public List<WaterMesh3D> Meshes;
        public Action<SpriteBatch, int> ReflectionCalls;
        public Action<SpriteBatch, int> SkyboxCall;

        public string Layer { get; set; } = "Water";

        public void AppendReflectionCall(Action<SpriteBatch, int> call) => ReflectionCalls += call;

        public void AppendSkyboxCall(Action<SpriteBatch, int> call) => SkyboxCall += call;

        public void Draw(SpriteBatch sb)
        {
            LinuxTechTips.DrawRectangle(new Rectangle(0, 100, 100, 100), Color.Red, 1);
            LinuxTechTips.DrawRectangle(new Rectangle(0, 200, 100, 100), Color.Red, 1);
            LinuxTechTips.DrawRectangle(new Rectangle(0, 300, 100, 100), Color.Red, 1);

            sb.Draw(AboveWater, new Rectangle(0, 100, 100, 100), Color.White);
            sb.Draw(BelowWater, new Rectangle(0, 200, 100, 100), Color.White);
            sb.Draw(ReverseOcclusionMap, new Rectangle(0, 300, 100, 100), Color.White);

            DrawWater(sb);
        }

        public void DrawInverseWaterMeshes(SpriteBatch sb)
        {
            foreach (WaterMesh3D Mesh in Meshes)
            {
                Mesh.DrawWaterInverse(sb);
            }
        }

        public void DrawWaterMeshes(SpriteBatch sb, Color color)
        {
            foreach (WaterMesh3D Mesh in Meshes)
            {
                Mesh.DrawWater(sb, color);
            }
        }

        public void DrawWaterMeshesEffect(SpriteBatch sb, Effect effect)
        {
            foreach (WaterMesh3D Mesh in Meshes)
            {
                Mesh.param = (e) =>
                {
                    DepthLayer currentlayer = DepthBuffer.GetLayer(Mesh.Layer);

                    e.Parameters["WorldViewProjection"].SetValue(currentlayer.Camera.ViewMatrix * currentlayer.Camera.ProjectionMatrix);
                    e.Parameters["dUdVMap"].SetValue(Asset.GetTexture(Asset.NoiseDirectory + "dUdVTexture"));
                    e.Parameters["progress"].SetValue(Main.GameUpdateCount / 1000f);
                    e.Parameters["coordDensity"].SetValue(new Vector2(3f, 3f));
                    e.Parameters["Target"].SetValue(currentlayer.Camera.Transform.Position);
                    e.Parameters["NormalMap"].SetValue(Asset.GetTexture(Asset.NoiseDirectory + "WaterNormalMap"));
                    e.Parameters["World"].SetValue(Matrix.Identity);
                    e.Parameters["SpecularPower"].SetValue(2f);
                    e.Parameters["LightDirection"].SetValue(new Vector3(0f, 1f, 0.5f));
                    e.Parameters["distortionCoefficient"].SetValue(0.01f);
                    e.Parameters["tint"].SetValue(new Vector4(0, 0.3f, 0.5f, 1.0f));
                    e.Parameters["lerptint"].SetValue(0.2f);
                    e.Parameters["reflectivity"].SetValue(0.6f);
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

            DepthBuffer.GetLayer("Seamap").DrawAllCalls(sb);

            sb.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(EntityMap);
            Main.graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil,
                 Color.Transparent, 0, 0);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            //DepthBuffer.GetLayer(Layer).DrawAllCalls(sb);
            ReflectionCalls?.Invoke(sb, 0);

            sb.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(ReverseEntityMap);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            DepthBuffer.GetLayer("Seamap").DrawAllCalls(sb);

            sb.End();

            DrawAboveWater(sb);
            DrawBelowWater(sb);

            Main.graphics.GraphicsDevice.SetRenderTarget(WaterTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            LinuxMod.dUdVMap.Parameters["reflectionMap"].SetValue(AboveWater);
            LinuxMod.dUdVMap.Parameters["refractionMap"].SetValue(BelowWater);

            DrawWaterMeshesEffect(sb, LinuxTechTips.GetScreenShader("dUdVMap").Shader);

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

            LinuxMod.WaterOcclusion.Parameters["occlusionTexture"].SetValue(ReverseOcclusionMap);
            LinuxMod.WaterOcclusion.Parameters["inverse"].SetValue(0);
            LinuxMod.WaterOcclusion.CurrentTechnique.Passes[0].Apply();

            //LinuxTechTips.GetScreenShader("PixelationShader").Shader.CurrentTechnique.Passes[0].Apply();
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

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

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

            LinuxMod.WaterOcclusion.Parameters["occlusionTexture"].SetValue(ReverseOcclusionMap);
            LinuxMod.WaterOcclusion.Parameters["inverse"].SetValue(1);
            LinuxMod.WaterOcclusion.CurrentTechnique.Passes[0].Apply();

            sb.Draw(ReverseEntityMap, OcclusionMap.Bounds, OcclusionMap.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            sb.End();
        }

        public void DrawWater(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            LinuxMod.WaterReflection.Parameters["oclMap"].SetValue(WaterOcclusion);
            LinuxMod.WaterReflection.CurrentTechnique.Passes[0].Apply();

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

            sb.Draw(EntityMap, new Rectangle(0, 100, 100, 100), Color.White);
            sb.Draw(BelowWater, new Rectangle(0, 200, 100, 100), Color.White);
            sb.Draw(EntityMap, new Rectangle(0, 300, 100, 100), Color.White);

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
            Meshes = new List<WaterMesh3D>();
        }

        public void Unload()
        {

        }
    }

    public class WaterMesh3D : QuadMesh
    {
        public Color InternalColor;

        public WaterMesh3D(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, WaterMeshContainer3D parent, Color color = default, string layer = "Default", Texture2D texture = null, Effect effect = null) :
            base(v1, v2, v3, v4, Color.Black, layer, texture, effect)
        {
            InternalColor = color;
            parent?.Meshes.Add(this);
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
            param?.Invoke(effect);

            vertexBuffer.SetData(vertices);
            indexBuffer.SetData(indices);

            sb.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            sb.GraphicsDevice.Indices = indexBuffer;

            sb.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            sb.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (EffectPass pass in LinuxMod.dUdVMap.CurrentTechnique.Passes)
            {
                pass.Apply();
                sb.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexPointer, 0, indexPointer / 3);
            }
        }
    }
}

