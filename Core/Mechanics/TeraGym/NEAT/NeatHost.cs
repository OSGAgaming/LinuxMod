using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics.TeraGym.NEAT
{
    public class NeatHost
    {
        public static readonly int MAX_NODES = (int)Math.Pow(2, 20);

        public Dictionary<ConnectionGene, ConnectionGene> allConnections = new Dictionary<ConnectionGene, ConnectionGene>();
        public List<NodeGene> allNodes = new List<NodeGene>();

        public int maxClients;
        public int outputSize;
        public int inputSize;

        public double C1, C2, C3 = 1;

        public double WEIGHT_SHIFT_STRENGTH = 0.3f;
        public double WEIGHT_RANDOM_STRENGTH = 1f;

        public double PROBABILITY_MUTATE_LINK = 0.4f;
        public double PROBABILITY_MUTATE_NODE = 0.4f;
        public double PROBABILITY_MUTATE_WEIGHT_SHIFT = 0.4f;
        public double PROBABILITY_MUTATE_WEIGHT_RANDOM = 0.4f;
        public double PROBABILITY_MUTATE_WEIGHT_TOGGLE_LINK = 0.4f;

        public NeatHost(int inputSize, int outputSize, int clients)
        {
            Reset(inputSize, outputSize, clients);
        }

        public Genome GenerateEmptyGenome()
        {
            Genome g = new Genome(this);
            for(int i = 0; i < inputSize + outputSize; i++)
            {
                g.nodes.Add(getNode(i + 1));
            }

            return g;
        }

        public void Reset(int inputSize, int outputSize, int clients)
        {
            this.inputSize = inputSize;
            this.outputSize = outputSize;
            maxClients = clients;

            allConnections.Clear();
            allNodes.Clear();

            for(int i = 0; i < inputSize; i++)
            {
                NodeGene n = getNode();
                n.x = 0.1f;
                n.y = (i + 1) / (float)(inputSize + 1);
            }

            for (int i = 0; i < outputSize; i++)
            {
                NodeGene n = getNode();
                n.x = 0.9f;
                n.y = (i + 1) / (float)(inputSize + 1);
            }
        }

        public NodeGene getNode()
        {
            NodeGene n = new NodeGene(allNodes.Count + 1);
            allNodes.Add(n);
            return n;
        }

        public NodeGene getNode(int id)
        {
            if (id <= allNodes.Count) return allNodes[id - 1];
            return getNode();
        }

        public static ConnectionGene getConnection(ConnectionGene connection)
        {
            ConnectionGene c = new ConnectionGene(connection.from, connection.to);
            c.weight = connection.weight;
            c.enabled = connection.enabled;

            return c;
        }

        public ConnectionGene getConnection(NodeGene node1, NodeGene node2)
        {
            ConnectionGene connectionGene = new ConnectionGene(node1, node2);

            if (allConnections.ContainsKey(connectionGene))
            {
                connectionGene.setInovationNumber(allConnections[connectionGene].getInovationNumber());
            }
            else
            {
                connectionGene.setInovationNumber(allConnections.Count + 1);
            }

            return connectionGene;
        }
    }
}