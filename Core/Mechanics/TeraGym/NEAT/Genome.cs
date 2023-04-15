using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics.TeraGym.NEAT
{
    public class Genome : IDna
    {
        public List<ConnectionGene> connections = new List<ConnectionGene>();
        public List<NodeGene> nodes = new List<NodeGene>();

        public NeatHost Neat;

        public Genome(NeatHost neat) => this.Neat = neat;

        public double Distance(Genome g2)
        {
            Genome g1 = this;

            int highInG1 = connections[g1.connections.Count - 1].getInovationNumber();
            int highInG2 = g2.connections[g2.connections.Count - 1].getInovationNumber();

            if(highInG1 < highInG2)
            {
                Genome g = g1;
                g1 = g2;
                g2 = g;
            }

            int i1 = 0;
            int i2 = 0;

            int disjoint = 0;
            int excess = 0;
            double weightDiff = 0;
            int similar = 0;

            while(i1 < g1.connections.Count && i2 < g2.connections.Count)
            {
                ConnectionGene gene1 = g1.connections[i1];
                ConnectionGene gene2 = g2.connections[i2];

                int in1 = gene1.getInovationNumber();
                int in2 = gene2.getInovationNumber();

                if(in1 == in2)
                {
                    i1++;
                    i2++;
                    similar++;
                    weightDiff += Math.Abs(gene1.weight - gene2.weight);
                }
                else if (in1 > in2)
                {
                    i2++;
                    disjoint++;
                }
                else
                {
                    i1++;
                    disjoint++;
                }
            }


            weightDiff /= similar;
            excess = g1.connections.Count - i1;

            double N = Math.Max(g1.connections.Count, g2.connections.Count);
            if (N < 20) N = 1;

            return disjoint / N + excess / N + weightDiff / N;
        }

        public IDna Combine(IDna combinee, float mutationRate)
        {
            Genome g1 = this;
            Genome g2 = combinee as Genome;

            NeatHost neat = g1.Neat;
            Genome genome = neat.GenerateEmptyGenome();

            int i1 = 0;
            int i2 = 0;

            while (i1 < g1.connections.Count && i2 < g2.connections.Count)
            {
                ConnectionGene gene1 = g1.connections[i1];
                ConnectionGene gene2 = g2.connections[i2];

                int in1 = gene1.getInovationNumber();
                int in2 = gene2.getInovationNumber();

                if (in1 == in2)
                {
                    if(Main.rand.NextFloat(1) > 0.5f)
                    {
                        genome.connections.Add(NeatHost.getConnection(gene1));
                    }
                    else
                    {
                        genome.connections.Add(NeatHost.getConnection(gene2));
                    }

                    i1++;
                    i2++;
                }
                else if (in1 > in2)
                {
                    //genome.connections.Add(NeatHost.getConnection(gene2));
                    i2++;
                }
                else
                {
                    genome.connections.Add(NeatHost.getConnection(gene1));
                    i1++;
                }
            }

            while(i1 < g1.connections.Count)
            {
                ConnectionGene gene1 = genome.connections[i1];
                genome.connections.Add(NeatHost.getConnection(gene1));
                i1++;
            }

            foreach(ConnectionGene c in genome.connections)
            {
                genome.nodes.Add(c.from);
                genome.nodes.Add(c.to);
            }

            return genome;
        }

        public void Mutate()
        {
            if (Neat.PROBABILITY_MUTATE_LINK > Main.rand.NextFloat()) MutateLink();
            if (Neat.PROBABILITY_MUTATE_NODE > Main.rand.NextFloat()) MutateNode();
            if (Neat.PROBABILITY_MUTATE_WEIGHT_SHIFT > Main.rand.NextFloat()) MutateWeightShift();
            if (Neat.PROBABILITY_MUTATE_WEIGHT_RANDOM > Main.rand.NextFloat()) MutateWieghtRandom();
            if (Neat.PROBABILITY_MUTATE_WEIGHT_TOGGLE_LINK > Main.rand.NextFloat()) MutateLinkToggle();

        }

        public void MutateLink()
        {
            for(int i = 0; i < 100; i++)
            {
                NodeGene a = nodes[Main.rand.Next(nodes.Count)];
                NodeGene b = nodes[Main.rand.Next(nodes.Count)];

                ConnectionGene con;
                if(a.x == b.x) continue;
                if (a.x < b.x)
                {
                    con = new ConnectionGene(a, b);
                }
                else
                {
                    con = new ConnectionGene(b, a);
                }

                if (connections.Contains(con)) continue;

                con = Neat.getConnection(con.from, con.to);
                con.weight = Main.rand.NextFloat(-1, 1) * Neat.WEIGHT_RANDOM_STRENGTH;

                connections.Add(con);
            }
        }

        public void MutateNode()
        {
            ConnectionGene con = connections[Main.rand.Next(connections.Count)];
            if (con == null) return;

            NodeGene from = con.from;
            NodeGene to = con.to;

            NodeGene middle = Neat.getNode();

            middle.x = (from.x + to.x) / 2f;
            middle.y = (from.y + to.y) / 2f + Main.rand.NextFloat(-0.03f,0.03f);

            ConnectionGene c1 = Neat.getConnection(from, middle);
            ConnectionGene c2 = Neat.getConnection(middle, to);

            c1.weight = 1;
            c2.weight = con.weight;
            c2.enabled = con.enabled;

            connections.Remove(con);
            connections.Add(c1);
            connections.Add(c2);

            nodes.Add(middle);
        }

        public void MutateWeightShift()
        {
            ConnectionGene con = connections[Main.rand.Next(connections.Count)];
            if(con != null)
            {
                con.weight += Main.rand.NextFloat(-1, 1) * Neat.WEIGHT_SHIFT_STRENGTH;
            }
        }

        public void MutateWieghtRandom()
        {
            ConnectionGene con = connections[Main.rand.Next(connections.Count)];
            if (con != null)
            {
                con.weight = Main.rand.NextFloat(-1, 1) * Neat.WEIGHT_RANDOM_STRENGTH;
            }
        }

        public void MutateLinkToggle()
        {
            ConnectionGene con = connections[Main.rand.Next(connections.Count)];
            if (con != null)
            {
                con.enabled = !con.enabled; 
            }
        }

    }
}