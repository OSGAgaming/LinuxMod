
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
                        Pass.RenderBatched(sb);
                        sb.End();

                        Pass.RenderPrimitive(sb);
                    }
                }

                i++;
            }

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets1);
        }

        public void OrderedShaderPass()
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
        }
        public void DrawToMap(string Map, MapRender MR) => MapPasses[Map].DrawToBatchedTarget(MR);

        public void AddMap(string MapName, MapPass MP) 
        {
            MP.Parent = this;
            MapPasses.Add(MapName, MP);
        }

        public MapPass Get(string MapName) => MapPasses[MapName];

        public MapPass Get<T>() where T : MapPass
        {
            //TODO: Support for multiple Passes with different ID's

            foreach(MapPass pass in MapPasses.Values)
            {
                if (pass is T) return (T)pass;
            }

            throw new System.Exception("Pass does not exist");
        }

    }
}