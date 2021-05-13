using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class Insignia : ISerializable
    {
        public IList<INode> Nodes = new List<INode>();
        public IList<INode> TempCompareNodes = new List<INode>();
        public IList<INode> TempErrorNodes = new List<INode>();

        public int InsigniaLength { get; }
        public float PerformanceIndicator { get; set; }

        public void Serialize(Stream stream)
        {
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            binaryWriter.Write(Nodes.Count);

            for (int i = 0; i < Nodes.Count; i++)
            {
                binaryWriter.WriteVector2(Nodes[i].Position);
                binaryWriter.Write(Nodes[i].Progression);
            }
        }

        public static Insignia Deserialize(Stream stream)
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            
            Insignia insg = new Insignia();
            int Count = binaryReader.ReadInt32();

            for (int i = 0; i < Count; i++)
            {
                insg.Nodes.Add(new INode(binaryReader.ReadVector2(), binaryReader.ReadSingle()));
            }

            return insg;
        }

        public void SetAndNormalizeNodes(List<INode> nodes)
        {
            Nodes.Clear();

            int InsigniaLength = nodes.Count;

            Vector4 boundHodlers = new Vector4(10000, -10000, 10000, -10000);

            foreach (INode node in nodes)
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

            for (int i = 0; i < InsigniaHost.ACCURACY; i++)
            {
                int CorrectI = (int)(i * (InsigniaLength / (float)InsigniaHost.ACCURACY));
                INode node = nodes[CorrectI];
                Vector2 Translation = new Vector2(boundHodlers.X, boundHodlers.Z);
                Vector2 NormPos = (node.Position - Translation) / new Vector2(boundHodlers.Y - boundHodlers.X, boundHodlers.W - boundHodlers.Z);
                Nodes.Add(new INode(NormPos, i));
            }
        }

        public void SetNodes(List<INode> nodes) => Nodes = nodes;

        public void DebugDraw(Vector2 position, float scale)
        {
            if(PerformanceIndicator > 0) PerformanceIndicator -= 0.01f;

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (i < Nodes.Count - 1)
                {
                    Vector2 p = position.ForDraw();
                    LUtils.DrawLine(p + Nodes[i].Position * scale, p + Nodes[i + 1].Position * scale, Color.Lerp(Color.Red, Color.Green, 1 - PerformanceIndicator));
                }
            }

        }

        public float CompareInsignia(Insignia insignia)
        {
            insignia.TempErrorNodes.Clear();
            IList<INode> CompareNodes = insignia.Nodes;

            float avgDist = 0;
            int biasAmount = 10;
            for (int i = 0; i < CompareNodes.Count; i++)
            {
                int nI = (int)((Nodes.Count / (float)CompareNodes.Count) * i);

                float biasCheck = Vector2.Distance(CompareNodes[i].Position, Nodes[nI].Position);
                insignia.TempErrorNodes.Add(new INode(Nodes[nI].Position, i));
                for (int j = -biasAmount; j < biasAmount + 1; j++)
                {
                    if (nI + j > 0 && nI + j < CompareNodes.Count)
                    {
                        float dist = Vector2.Distance(CompareNodes[i].Position, Nodes[nI + j].Position);
                        if (dist < biasCheck)
                        {
                            biasCheck = dist;
                            //insignia.TempErrorNodes[insignia.TempErrorNodes.Count - 1] = new INode(Nodes[nI + j].Position, i);
                        }
                    }
                }
                avgDist += biasCheck;
            }
            return avgDist / CompareNodes.Count;
        }
    }
}