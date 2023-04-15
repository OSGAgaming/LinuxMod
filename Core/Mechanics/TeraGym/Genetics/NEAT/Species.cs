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
    public class Species
    {
        public List<NeatAgent> clients = new List<NeatAgent>();
        public NeatAgent representative;
        public double score;

        public Species(NeatAgent representative)
        {
            this.representative = representative;
            this.representative.SetSpecies(this);   
            clients.Add(representative); 
        }

        public bool Add(NeatAgent agent)
        {
            if (agent.Distance(representative) < representative.GetGenome().Neat.CP)
            {
                agent.SetSpecies(this);
                clients.Add(agent);
                return true;
            }
            return false;
        }

        public void ForceAdd(NeatAgent agent)
        {
            agent.SetSpecies(this);
            clients.Add(agent);
        }

        public void GoExtinct()
        {
            foreach(NeatAgent c in clients)
            {
                c.SetSpecies(null);
            }
        }

        public void EvaluateScore()
        {
            double v = 0;
            foreach(NeatAgent a in clients)
            {
                v += a.Fitness;
            }
            score = v / clients.Count;
        }

        public void Reset()
        {
            representative = clients[Main.rand.Next(clients.Count)];
            foreach(NeatAgent c in clients)
            {
                c.SetSpecies(null);
            }

            clients.Clear();

            clients.Add(representative);
            representative.SetSpecies(this);

            score = 0;
        }

        public void Kill(float percentage)
        {
            clients.Sort((x, y) => Math.Sign(x.Fitness - y.Fitness));

            double amount = percentage * clients.Count;
            for (int i = 0; i < amount; i++)
            {
                clients[0].SetSpecies(null);
                clients.RemoveAt(0);
            }
        }

        public Genome Breed()
        {
            NeatAgent a1 = clients[Main.rand.Next(clients.Count)];
            NeatAgent a2 = clients[Main.rand.Next(clients.Count)];

            if (a1.Fitness > a2.Fitness) return (Genome)a1.GetGenome().Combine(a2.GetGenome(), 0);

            return (Genome)a2.GetGenome().Combine(a1.GetGenome(), 0);
        }

        public int Size() => clients.Count;
    }
}