using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
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
        private GenomeCalculator calculator;

        public Genome(NeatHost neat) => this.Neat = neat;

        public float[] Response => calculator.GetOutputs();

        public void GenerateCalculator() => calculator = new GenomeCalculator(this);

        public double[] Calculate(params float[] input)
        {
            if (calculator == null) GenerateCalculator();
            return calculator.Calculate(input);
        }

        public void Compute(float[] input)
        {
            Calculate(input);
        }

        public double Distance(Genome g2)
        {
            Genome g1 = this;

            int highInG1 = 0;
            if (g1.connections.Count != 0) highInG1 = g1.connections[g1.connections.Count - 1].getInovationNumber();
            int highInG2 = 0;
            if (g2.connections.Count != 0) highInG2 = g2.connections[g2.connections.Count - 1].getInovationNumber();

            if (highInG1 < highInG2)
            {
                Genome g = g1;
                g1 = g2;
                g2 = g;
            }

            int i1 = 0;
            int i2 = 0;

            int disjoint = 0;
            int excess;
            double weightDiff = 0;
            int similar = 0;

            while (i1 < g1.connections.Count && i2 < g2.connections.Count)
            {
                ConnectionGene gene1 = g1.connections[i1];
                ConnectionGene gene2 = g2.connections[i2];

                int in1 = gene1.getInovationNumber();
                int in2 = gene2.getInovationNumber();

                if (in1 == in2)
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


            weightDiff /= Math.Max(1, similar);
            excess = g1.connections.Count - i1;

            double N = Math.Max(g1.connections.Count, g2.connections.Count);
            if (N < 20) N = 1;

            return (disjoint * Neat.C1) / N + (excess * Neat.C2) / N + weightDiff * Neat.C3;
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
                    if (Main.rand.NextFloat(1) > 0.5f)
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

            while (i1 < g1.connections.Count)
            {
                ConnectionGene gene1 = g1.connections[i1];
                genome.connections.Add(NeatHost.getConnection(gene1));
                i1++;
            }

            foreach (ConnectionGene c in genome.connections)
            {
                if (!genome.nodes.Contains(c.from)) genome.nodes.Add(c.from);
                if (!genome.nodes.Contains(c.to)) genome.nodes.Add(c.to);
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
            for (int i = 0; i < 100; i++)
            {
                NodeGene a = nodes[Main.rand.Next(nodes.Count)];
                NodeGene b = nodes[Main.rand.Next(nodes.Count)];

                ConnectionGene con;
                if (a.x == b.x) continue;
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
                connections.Sort((x, y) => x.getInovationNumber() - y.getInovationNumber());
                return;
            }
        }

        public void MutateNode()
        {
            if (connections.Count == 0) return;
            ConnectionGene con = connections[Main.rand.Next(connections.Count)];
            if (con == null) return;

            NodeGene from = con.from;
            NodeGene to = con.to;

            NodeGene middle = Neat.getNode();

            middle.x = (from.x + to.x) / 2f;
            middle.y = (from.y + to.y) / 2f + Main.rand.NextFloat(-0.03f, 0.03f);

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
            if (connections.Count == 0) return;
            ConnectionGene con = connections[Main.rand.Next(connections.Count)];
            if (con != null)
            {
                con.weight += Main.rand.NextFloat(-1, 1) * Neat.WEIGHT_SHIFT_STRENGTH;
            }
        }

        public void MutateWieghtRandom()
        {
            if (connections.Count == 0) return;
            ConnectionGene con = connections[Main.rand.Next(connections.Count)];
            if (con != null)
            {
                con.weight = Main.rand.NextFloat(-1, 1) * Neat.WEIGHT_RANDOM_STRENGTH;
            }
        }

        public void MutateLinkToggle()
        {
            if (connections.Count == 0) return;
            ConnectionGene con = connections[Main.rand.Next(connections.Count)];
            if (con != null)
            {
                con.enabled = !con.enabled;
            }
        }

        public void Draw(SpriteBatch sb, Vector2 position)
        {
            int size = 200;

            foreach (NodeGene n in nodes)
            {
                LinuxTechTips.DrawCircle(position + new Vector2(size) * new Vector2(n.x, n.y), new Vector2(10), Color.Lerp(Color.AliceBlue, Color.Purple, 0));
            }

            foreach (ConnectionGene c in connections)
            {
                if (!c.enabled) continue;

                Vector2 p1 = position + new Vector2(size) * new Vector2(c.to.x, c.to.y);
                Vector2 p2 = position + new Vector2(size) * new Vector2(c.from.x, c.from.y);

                LinuxTechTips.DrawLine(p1, p2, Color.Lerp(Color.MediumVioletRed, Color.Goldenrod, (float)c.weight));
            }
        }
    }
}