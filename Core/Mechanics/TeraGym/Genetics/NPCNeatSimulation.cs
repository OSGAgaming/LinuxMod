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
    public class NPCNeatSimulation<T> : NEATSimulation where T : NPCNeatAgent, new()
    {
        public Func<IDna, T> GenerateAgentWithDNA;
        public Vector2 DrawPosition => new Vector2(150, 200);
        public int Size => 300;


        public NPCNeatSimulation(
            int inputSize,
            int outputSize,
            int GenerationSize,
            Func<IDna, T> GenerateAgentWithDNA,
            int MaxSimulationTime = 1) :
            base(inputSize, outputSize, GenerationSize, 0, MaxSimulationTime)
        {
            this.GenerateAgentWithDNA = GenerateAgentWithDNA;
        }

        public override GeneticAgent InitialiseAgent() => new T();

        public override GeneticAgent InitialiseAgent(IDna dna) => GenerateAgentWithDNA.Invoke(dna);

        public override void Draw(SpriteBatch sb)
        {
            if (BestAgent == null) return;

            if (BestAgent is NPCNeatAgent n)
            {
                Genome network = n.Entity.network as Genome;

                network.Draw(sb, DrawPosition, Size);

                LinuxTechTips.UITextToLeft("Best Neural Net: ", Color.Black, DrawPosition, 1);
                LinuxTechTips.UITextToLeft("Population Size: " + GenerationSize.ToString(), Color.Black, DrawPosition + new Vector2(0, Size), 1);
                LinuxTechTips.UITextToLeft("Generation: " + Generation.ToString(), Color.Black, DrawPosition + new Vector2(0, Size + 50), 1);
                LinuxTechTips.UITextToLeft("Best Fitness: " + n.Fitness.ToString(), Color.Black, DrawPosition + new Vector2(0, Size + 100), 1);
                LinuxTechTips.UITextToLeft("Number of Species: " + neatHost.species.Count, Color.Black, DrawPosition + new Vector2(0, Size + 150), 1);

                LinuxTechTips.DrawCircle(n.Entity.Center.ForDraw(), new Vector2(40), Color.Gold * 0.3f);
            }

            foreach (GeneticAgent a in Agents)
            {
                NPCNeatAgent agent = a as NPCNeatAgent;
                if (!agent.IsActive()) continue;

                int h = agent.GetSpecies().GetHashCode();
                LinuxTechTips.DrawBoxFill(agent.Entity.position.ForDraw() - new Vector2(20), 10, 10, new Color(h % 255, (h % 120) * 2, (h % 15) * 21));

                //Genome network = (agent.Entity as Agent)?.network as Genome;
                //network.Draw(sb, agent.Entity.position.ForDraw() - new Vector2(-50,50), 50);
            }
        }

    }
}