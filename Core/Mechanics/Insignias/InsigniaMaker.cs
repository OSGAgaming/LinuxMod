using LinuxMod.Core.Helper.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class InsigniaHost
    {
        public List<PNode> CurrentNodeCache = new List<PNode>();
        public List<PNode> CurrentNode = new List<PNode>();
        public List<PNode> TestNodes = new List<PNode>();

        public Vector2 CurrentNodeAnchor;

        public int InsigniaLengthTrack;
        public int InsigniaLength;
        private const int ACCURACY = 50;

        public bool MouseState = Main.LocalPlayer.controlUseItem;
        float lerp;
        public void Update()
        {
            bool JustLiftedMouse = MouseState && !Main.LocalPlayer.controlUseItem;

            lerp *= 0.98f;

            if (TestNodes.Count > 1)
            {
                for (int i = 0; i < TestNodes.Count - 1; i++)
                {
                    Vector2 p = Main.LocalPlayer.Center.ForDraw();
                    if (lerp > 0)
                        LUtils.DrawLine(p + TestNodes[i].Position * 100 + new Vector2(100, 0), p + TestNodes[i + 1].Position * 100 + new Vector2(100, 0), Color.Lerp(Color.White, Color.Green, lerp));
                    else
                    {
                        LUtils.DrawLine(p + TestNodes[i].Position * 100 + new Vector2(100, 0), p + TestNodes[i + 1].Position * 100 + new Vector2(100, 0), Color.Lerp(Color.White, Color.Red, -lerp));
                    }
                }
            }

            if (Main.LocalPlayer.controlUseItem)
            {
                if (InsigniaLengthTrack == 0)
                {
                    CurrentNodeCache.Clear();
                    CurrentNode.Clear();
                    CurrentNodeAnchor = Main.MouseScreen;
                }

                CurrentNodeCache.Add(new PNode(Main.MouseScreen - CurrentNodeAnchor, InsigniaLengthTrack));

                for (int i = 0; i < CurrentNodeCache.Count - 1; i++)
                {
                    Vector2 p = Main.LocalPlayer.Center.ForDraw();
                    LUtils.DrawLine(p + CurrentNodeCache[i].Position + new Vector2(100,0), p + CurrentNodeCache[i + 1].Position + new Vector2(100, 0), Color.LightGoldenrodYellow);
                }

                InsigniaLengthTrack++;
            }
            else if (JustLiftedMouse && InsigniaLengthTrack > ACCURACY)
            {
                InsigniaCreation();
            }
            else
            {
                InsigniaLengthTrack = 0;
            }

            MouseState = Main.LocalPlayer.controlUseItem;
        }

        public void InsigniaCreation()
        {
            InsigniaLength = InsigniaLengthTrack;

            Vector4 boundHodlers = new Vector4(10000, -10000, 10000, -10000);

            foreach (PNode node in CurrentNodeCache)
            {
                if (node.Position.X < boundHodlers.X)
                    boundHodlers.X = node.Position.X;
                if (node.Position.X > boundHodlers.Y)
                    boundHodlers.Y = node.Position.X;
                if (node.Position.Y < boundHodlers.Z)
                    boundHodlers.Z = node.Position.Y;
                if (node.Position.Y > boundHodlers.W)
                    boundHodlers.W = node.Position.Y;
            }

            for (int i = 0; i < ACCURACY; i++)
            {
                int CorrectI = (int)(i * (InsigniaLength / (float)ACCURACY));
                PNode node = CurrentNodeCache[CorrectI];
                Vector2 NormPos = node.Position / new Vector2(boundHodlers.Y - boundHodlers.X, boundHodlers.W - boundHodlers.Z);
                CurrentNode.Add(new PNode(NormPos, i));
            }

            if (TestNodes.Count == 0)
            {
                for (int i = 0; i < ACCURACY; i++)
                {
                    TestNodes.Add(CurrentNode[i]);
                }
            }
            else
            {
                float avgDist = 0;
                int biasAmount = 4;
                for (int i = 0; i < ACCURACY; i++)
                {
                    float biasCheck = Vector2.Distance(TestNodes[i].Position, CurrentNode[i].Position);
                    for (int j = -biasAmount; j < biasAmount + 1; j++)
                    {
                        if (i + j > 0 && i + j < ACCURACY)
                        {
                            float dist = Vector2.Distance(TestNodes[i].Position, CurrentNode[i + j].Position);
                            if (dist < biasCheck)
                            {
                                biasCheck = dist;
                            }
                        }
                    }
                    avgDist += biasCheck;
                }
                if (avgDist / ACCURACY < 0.2f) lerp = 1;
                else lerp = -1;
            }
        }
    }
}