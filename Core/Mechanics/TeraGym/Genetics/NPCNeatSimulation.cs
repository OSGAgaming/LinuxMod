using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System.Collections.Generic;
using System;
using LinuxMod.Content.NPCs.Genetics;
using System.Data;
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.TeraGym.NEAT;

namespace LinuxMod.Core.Mechanics
{
    public class NPCNeatSimulation<T> : NEATSimulation where T : NPCNeatAgent
    {
        public int Type = 0;
        public Func<IDna, int, T> GenerateAgentWithDNA;
        public Func<int, T> GenerateAgent;

        public NPCNeatSimulation(
            int inputSize,
            int outputSize,
            int Type, 
            int GenerationSize, 
            Func<IDna, int, T> GenerateAgentWithDNA,
            Func<int, T> GenerateAgent,
            float MutationRate = 0.01F, 
            int MaxSimulationTime = 1) : 
            base(inputSize, outputSize, GenerationSize, MutationRate, MaxSimulationTime)
        {
            this.Type = Type;
            this.GenerateAgentWithDNA = GenerateAgentWithDNA;
            this.GenerateAgent = GenerateAgent;
        }

        public override GeneticAgent InitialiseAgent() => GenerateAgent.Invoke(Type);
        
        public override GeneticAgent InitialiseAgent(IDna dna) => GenerateAgentWithDNA.Invoke(dna, Type);

        public override void Draw(SpriteBatch sb)
        {
            if (BestAgent == null) return;

            if (BestAgent is NPCNeatAgent n)
            {
                Genome network = ((BestAgent as NPCNeatAgent)?.Entity.modNPC as ExampleAgent)?.network as Genome;
                network.Draw(sb, new Vector2(200));
                LinuxTechTips.UITextToCenter("Best Neural Net: ", Color.Black, new Vector2(250, 100), 1);
                LinuxTechTips.UITextToCenter("Population Size: " + GenerationSize.ToString(), Color.Black, new Vector2(250,500), 1);
                LinuxTechTips.UITextToCenter("Generation: " + Generation.ToString(), Color.Black, new Vector2(250, 550), 1);
                LinuxTechTips.UITextToCenter("Mutation Rate: " + MutationRate.ToString(), Color.Black, new Vector2(250, 600), 1);
                LinuxTechTips.UITextToCenter("Best Fitness: " + n.Fitness.ToString(), Color.Black, new Vector2(250, 650), 1);

                LinuxTechTips.DrawCircle(n.Entity.Center, new Vector2(10), Color.Gold);
                float[] output = network.Response;

                for(int i = 0; i < output.Length; i++)
                {
                    Main.NewText(i + ": " + output[i]);
                }
            }
        }

    }
}