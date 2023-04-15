using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics.TeraGym.NEAT
{
    public class GenomeCalculator
    {
        public List<NeatNode> inputNodes = new List<NeatNode>();
        public List<NeatNode> hiddenNodes = new List<NeatNode>();
        public List<NeatNode> outputNodes = new List<NeatNode>();

        public GenomeCalculator(Genome g)
        {
            List<NodeGene> nodes = g.nodes;
            List<ConnectionGene> cons = g.connections;

            Dictionary<int, NeatNode> nodeHashMap = new Dictionary<int, NeatNode>();

            foreach(NodeGene n in nodes)
            {
                NeatNode node = new NeatNode(n.x);
                nodeHashMap.Add(n.getInovationNumber(), node);

                if(n.x <= 0.1f)
                {
                    inputNodes.Add(node);
                } else if(n.x <= 0.9f)
                {
                    outputNodes.Add(node);
                }
                else
                {
                    hiddenNodes.Add(node);
                }
            }

            hiddenNodes.Sort();

            foreach(ConnectionGene c in cons)
            {
                NodeGene from = c.from;
                NodeGene to = c.to;

                NeatNode nFrom = nodeHashMap[from.getInovationNumber()];
                NeatNode nTo = nodeHashMap[to.getInovationNumber()];

                Connection con = new Connection(nFrom, nTo);
                con.weight = c.weight;
                con.enabled = c.enabled;

                nTo.connections.Add(con);
            }
        }

        public double[] Calculate(params float[] input)
        {
            if (input.Length != inputNodes.Count) throw new System.Exception("Input structure does not match");
            for(int i = 0; i < inputNodes.Count; i++)
            {
                inputNodes[i].value = input[i];
            }
            foreach(NeatNode n in hiddenNodes)
            {
                n.Calculate();
            }

            double[] output = new double[outputNodes.Count]; 
            for(int i = 0; i < outputNodes.Count; i++)
            {
                outputNodes[i].Calculate();
                output[i] = outputNodes[i].value;
            }

            return output;
        }

        public float[] GetOutputs()
        {
            float[] values = new float[outputNodes.Count];
            for(int i = 0; i < outputNodes.Count; i++)
            {
                values[i] = outputNodes[i].value;
            }

            return values;
        }
    }
}