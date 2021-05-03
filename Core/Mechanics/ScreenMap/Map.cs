
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;

namespace LinuxMod.Core.Mechanics.ScreenMap
{
    public class Map
    {
        internal Dictionary<string, MapPass> MapPasses = new Dictionary<string, MapPass>();

        public void OrderedRenderPassBatched(SpriteBatch sb, GraphicsDevice GD)
        {
            RenderTargetBinding[] oldtargets1 = Main.graphics.GraphicsDevice.GetRenderTargets();
            int i = 0;

            Matrix matrix = Main.GameViewMatrix.TransformationMatrix;

            for (int a = 0; a < MapPasses.Count; a++)
            {
                foreach (KeyValuePair<string, MapPass> Map in MapPasses)
                {
                    var Pass = Map.Value;

                    if (Pass.Index != i) continue;
                    Matrix ScalieMatrix = Matrix.CreateScale(1/ (float)Pass.PixelationFactor);

                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, matrix);
                    Pass.DrawToPixelationTarget(sb, GD);
                    sb.End();

                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, matrix);
                    Pass.Render(sb, GD);
                    sb.End();
                }

                i++;
            }

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets1);
        }

        public void OrderedRenderPass(SpriteBatch sb, GraphicsDevice GD)
        {
            RenderTargetBinding[] oldtargets1 = Main.graphics.GraphicsDevice.GetRenderTargets();

            int i = 0;
            sb.End();
            foreach (KeyValuePair<string, MapPass> Map in MapPasses)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                var Pass = Map.Value;

                if (Pass.Index == i) Pass.Render(sb, GD);

                i++;

                sb.End();
            }

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets1);

        }

        public List<RenderTarget2D> Buffers = new List<RenderTarget2D>();

        public RenderTarget2D OrderedShaderPass()
        {
            int i = 0;

            for (int a = 0; a < MapPasses.Count; a++)
            {
                foreach (KeyValuePair<string, MapPass> Map in MapPasses)
                {
                    var Pass = Map.Value;

                    if (Pass.Index != i) continue;

                    Pass.ApplyShader();
                }

                i++;
            }

            return Buffers[Buffers.Count - 1];
        }
        public void DrawToMap(string Map, MapRender MR) => MapPasses[Map].DrawToTarget(MR);

        public void AddMap(string MapName,int Index, MapPass MP) 
        {
            MP.Parent = this;
            MP.Index = Index;
            MapPasses.Add(MapName, MP);
            Buffers.Add(new RenderTarget2D(Main.graphics.GraphicsDevice,2560,1440));
        }

        public MapPass Get(string MapName) => MapPasses[MapName];
    }
}