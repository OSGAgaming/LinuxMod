
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using EEMod.Extensions;
using System.Linq;
using System;
using static Terraria.ModLoader.ModContent;
using System.Reflection;
using LinuxMod.Core.Mechanics.Interfaces;

namespace LinuxMod.Core.Mechanics.Primitives
{
    public class IndexedPrimitive : ILayeredDraw
    {
        protected VertexPositionColorTexture[] vertices;
        protected short[] indices;
        protected int vertexPointer;
        protected int indexPointer;
        private bool finished;
        protected string layer;
        private Texture2D texture;

        protected Effect effect;
        public Action<Effect> param;
        protected BasicEffect basicEffect;

        protected VertexBuffer vertexBuffer;
        protected IndexBuffer indexBuffer;
        protected VertexBuffer normalVertexBuffer;

        public string Layer => layer;

        public IndexedPrimitive(int vertexCount, int indexCount, string layer, Texture2D texture = null, Effect effect = null, Action<Effect> param = null)
        {
            vertices = new VertexPositionColorTexture[vertexCount];
            indices = new short[indexCount];
            this.layer = layer;
            this.effect = effect;
            this.texture = texture;
            this.param = param;
            basicEffect = new BasicEffect(Main.graphics.GraphicsDevice);

            normalVertexBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.WriteOnly);
            vertexBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionColorTexture), vertexCount, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(Main.graphics.GraphicsDevice, typeof(short), indexCount, BufferUsage.WriteOnly);
        }

        public void AddVertex(Vector3 position, Color color, Vector2 uv)
        {
            ResetIfFinished();

            vertices[vertexPointer++] = new VertexPositionColorTexture(position, color, uv);

            if (vertexPointer == vertices.Length + 1)
                Array.Resize(ref vertices, vertices.Length * 2);
        }

        public void AddIndex(short index)
        {
            ResetIfFinished();

            indices[indexPointer++] = index;

            if (indexPointer == indices.Length + 1)
                Array.Resize(ref indices, indices.Length * 2);
        }

        public void Finish() => finished = true;

        public void Reset()
        {
            Array.Clear(vertices, 0, vertices.Length);
            Array.Clear(indices, 0, indices.Length);
            vertexPointer = 0;
            indexPointer = 0;
        }

        private void ResetIfFinished()
        {
            if (finished)
            {
                finished = false;
                Reset();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            DepthLayer currentlayer = DepthBuffer.GetLayer(layer);

            basicEffect.TextureEnabled = texture != null;
            basicEffect.Texture = texture;
            basicEffect.VertexColorEnabled = true;
            basicEffect.FogColor = Color.AliceBlue.ToVector3();
            basicEffect.FogStart = 20;
            basicEffect.FogEnd = 1000;

            basicEffect.View = currentlayer.Camera.ViewMatrix;
            basicEffect.Projection = currentlayer.Camera.ProjectionMatrix;
            basicEffect.World = currentlayer.Camera.WorldMatrix;

            vertexBuffer.SetData(vertices);
            indexBuffer.SetData(indices);

            sb.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            sb.GraphicsDevice.Indices = indexBuffer;

            if (effect == null)
            {
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    sb.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexPointer, 0, indexPointer / 3);
                }
            }
            else
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    param.Invoke(effect);

                    pass.Apply();
                    sb.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexPointer, 0, indexPointer / 3);
                }
            }
        }
    }
}