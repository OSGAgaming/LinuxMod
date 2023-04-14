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

namespace LinuxMod.Core.Mechanics
{
    public class GeneticAgent
    {
        public IDna Dna;
        public float Fitness;

        public GeneticAgent(IDna Dna)
        {
            this.Dna = Dna;
        }

        public GeneticAgent()
        {
            GenerateRandomAgent();
        }

        public virtual void GenerateRandomAgent() { }
        public virtual void CalculateFitness() { }
    }

    public class RuntimeGeneticAgent : GeneticAgent
    {
        public RuntimeGeneticAgent(IDna Dna) : base(Dna)
        {
        }

        public virtual void CalculateContinousFitness() { }
    }
}