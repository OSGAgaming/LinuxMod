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
using System.Data;

namespace LinuxMod.Core.Mechanics
{
    public class NPCSimulation<T> : Simulation where T : NPCAgent
    {
        public int Type = 0;
        public Func<IDna, int, T> GenerateAgentWithDNA;
        public Func<int, T> GenerateAgent;

        public NPCSimulation(
            int Type, 
            int GenerationSize, 
            Func<IDna, int, T> GenerateAgentWithDNA,
            Func<int, T> GenerateAgent,
            float MutationRate = 0.01F, 
            int MaxSimulationTime = 1) : 
            base(GenerationSize, MutationRate, MaxSimulationTime)
        {
            this.Type = Type;
            this.GenerateAgentWithDNA = GenerateAgentWithDNA;
            this.GenerateAgent = GenerateAgent;
        }

        public override GeneticAgent InitialiseAgent()
        {
            return GenerateAgent.Invoke(Type);
        }
        public override GeneticAgent InitialiseAgent(IDna dna)
        {
            return GenerateAgentWithDNA.Invoke(dna, Type);
        }
    }
}