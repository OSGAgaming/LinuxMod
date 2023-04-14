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
    public class ExampleNPCAgent : NPCAgent
    {
        public ExampleNPCAgent(IDna Dna, int type) : base(Dna, type) { }

        public ExampleNPCAgent(int type) : base(type) { }

        public override IDna GenerateRandomAgent()
        {
            IDna network = new BaseNeuralNetwork(13)
                   .AddLayer<SigmoidActivationFunction>(17)
                   .AddLayer<SigmoidActivationFunction>(17)
                   .SetOutput<SigmoidActivationFunction>(4)
                   .GenerateWeights(() => Main.rand.NextFloat(-1, 1));

            return network;
        }

        public override void CalculateCurrentFitness()
        {
            float distance = Math.Max(0.1f, Vector2.Distance(Main.LocalPlayer.position, Entity.position));
            Fitness += 1f / distance;
            Fitness = Math.Max(Fitness, 0);
        }

        public override void CalculateContinousFitness()
        {
            float distance = Math.Max(0.1f, Vector2.Distance(Main.LocalPlayer.position, Entity.position));
            //Fitness += (1f / distance) * 0.01f;
        }
    }
}