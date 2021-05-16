
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

        public void OrderedRenderPassBatched(SpriteBatch sb, GraphicsDevice GD, bool Batched = true)
        {
            RenderTargetBinding[] oldtargets1 = Main.graphics.GraphicsDevice.GetRenderTargets();
            int i = 0;

            Matrix matrix = Main.GameViewMatrix.ZoomMatrix;

            for (int a = 0; a < MapPasses.Count; a++)
            {
                foreach (KeyValuePair<string, MapPass> Map in MapPasses)
                {
                    var Pass = Map.Value;

                    if (Pass.Priority != i) continue;

                    if (Pass.ManualTarget == null)
                    {
                        GD.SetRenderTarget(Pass.MapTarget);
                        GD.Clear(Color.Transparent);

                        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, matrix);
                        Pass.RenderBatched(sb, GD);
                        sb.End();
                        Pass.RenderPrimitive(sb, GD);
                    }
                }

                i++;
            }

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets1);
        }

        public void OrderedRenderPass(SpriteBatch sb, GraphicsDevice GD, bool Batched = true)
        {
            RenderTargetBinding[] oldtargets1 = Main.graphics.GraphicsDevice.GetRenderTargets();

            int i = 0;
           
            foreach (KeyValuePair<string, MapPass> Map in MapPasses)
            {
                
                var Pass = Map.Value;

                if (Pass.Priority == i)
                {
                    if (Batched) Pass.RenderBatched(sb, GD);
                    else Pass.RenderPrimitive(sb, GD);
                }

                i++;
            }


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

                    if (Pass.Priority != i) continue;

                    Pass.ApplyShader();
                }

                i++;
            }

            return Buffers[Buffers.Count - 1];
        }
        public void DrawToMap(string Map, MapRender MR) => MapPasses[Map].DrawToBatchedTarget(MR);

        public void AddMap(string MapName, MapPass MP) 
        {
            MP.Parent = this;
            MapPasses.Add(MapName, MP);
            Buffers.Add(new RenderTarget2D(Main.graphics.GraphicsDevice,2560,1440));
        }

        public MapPass Get(string MapName) => MapPasses[MapName];
    }
}