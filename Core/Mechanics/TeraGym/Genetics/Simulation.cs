using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System.Collections.Generic;
using System;
using LinuxMod.Content.NPCs.Genetics;

namespace LinuxMod.Core.Mechanics
{
    public class Simulation
    {
        public List<GeneticAgent> Agents = new List<GeneticAgent>();
        public GeneticAgent BestAgent;

        public int GenerationSize;
        public int MaxSimulationTime;

        public float MutationRate;
        public int Time;

        public int Generation;

        public Simulation(int GenerationSize, float MutationRate = 0.01f, int MaxSimulationTime = 1)
        {
            this.GenerationSize = GenerationSize;
            this.MutationRate = MutationRate;
            this.MaxSimulationTime = MaxSimulationTime;
        }

        public virtual GeneticAgent InitialiseAgent() { return new GeneticAgent(); }
        public virtual GeneticAgent InitialiseAgent(IDna dna) { return new GeneticAgent(dna); }

        public virtual void Deploy()
        {
            for (int i = 0; i < GenerationSize; i++)
            {
                Agents.Add(InitialiseAgent());
            }
        }

        public GeneticAgent PickFitnessWeightedAgent()
        {
            float totalFitness = 0;

            foreach (GeneticAgent agent in Agents) totalFitness += agent.Fitness;
            
            float r = Main.rand.NextFloat(totalFitness);
            int index = 0;
            while(r > 0)
            {
                r -= Agents[index].Fitness;
                index++;
            }

            return Agents[index - 1];
        }

        public List<GeneticAgent> GenerateFitnessWeightedPopulation()
        {
            List<GeneticAgent> agents = new List<GeneticAgent>();
            for(int i = 0; i < GenerationSize; i++)
            {
                IDna newDNA = PickFitnessWeightedAgent().Dna.Combine(PickFitnessWeightedAgent().Dna, MutationRate);
                agents.Add(InitialiseAgent(newDNA));
            }

            return agents;
        }

        public GeneticAgent FindBestAgent()
        {
            float max = float.MinValue;
            GeneticAgent agent = null;

            foreach (GeneticAgent a in Agents)
            {
                if (a.Fitness > max)
                {
                    max = a.Fitness;
                    agent = a;
                }
            }

            return agent;
        }

        public void Destroy()
        {
            foreach (GeneticAgent agent in Agents)
            {
                if (agent is ContinuousGeneticAgent r) r.Kill();
            }

            Agents.Clear();
        }

        public virtual void Update()
        {
            Time++;
            int inActivity = 0;

            foreach(GeneticAgent agent in Agents)
            {
                if (agent is ContinuousGeneticAgent r)
                {
                    r.Update();
                    if (!r.IsActive()) inActivity++;
                }
                else
                {
                    inActivity++;
                }
            }

            if(Time >= MaxSimulationTime || inActivity == Agents.Count)
            {  
                foreach (GeneticAgent agent in Agents)
                {
                    if (agent is ContinuousGeneticAgent r)
                    {
                        if(r.IsActive()) r.Kill();
                    }
                    else agent.CalculateCurrentFitness();
                }

                BestAgent = FindBestAgent();

                List<GeneticAgent> newPop = GenerateFitnessWeightedPopulation();
                Agents = newPop;
                Time = 0;
                Generation++;
            }
        }

        public virtual void Draw(SpriteBatch sb) { }
    }
}