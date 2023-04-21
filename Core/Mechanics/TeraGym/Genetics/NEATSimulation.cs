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
using LinuxMod.Core.Mechanics.TeraGym.NEAT;

namespace LinuxMod.Core.Mechanics
{
    public class NEATSimulation : Simulation
    {
        public NeatHost neatHost;
        private int inputSize;
        private int outputSize;
        private int maxClients;
        private bool isSimulating;

        public NEATSimulation(int inputSize, int outputSize, int GenerationSize, float MutationRate = 0.01F, int MaxSimulationTime = 1) : base(GenerationSize, MutationRate, MaxSimulationTime)
        {
            this.inputSize = inputSize;
            this.outputSize = outputSize;
            maxClients = GenerationSize;

            neatHost = new NeatHost(inputSize, outputSize, GenerationSize, this);
        }
        public override void Deploy() 
        { 
            neatHost.Reset(inputSize,outputSize, maxClients);
        }
        public override void Update()
        {
            Time++;
            int inActivity = 0;

            foreach (GeneticAgent agent in Agents)
            {
                if (agent is ContinuousGeneticAgent r)
                {
                    if (!r.IsActive()) inActivity++;
                    else r.Update();
                }
                else
                {
                    inActivity++;
                }
            }


            if (Time >= MaxSimulationTime || inActivity == Agents.Count)
            {
                foreach (GeneticAgent agent in Agents)
                {
                    if (agent is ContinuousGeneticAgent r)
                    {
                        if (r.IsActive()) r.Kill();
                    }
                    else agent.CalculateCurrentFitness();
                }


                BestAgent = FindBestAgent();
                neatHost.Evolve();
                Time = 0;
                Generation++;
            }
        }
    }
}