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
            Dna = GenerateRandomAgent();
        }

        public virtual IDna GenerateRandomAgent() { return null; }
        public virtual void CalculateCurrentFitness() { }
    }

    public class ContinuousGeneticAgent : GeneticAgent
    {
        private bool Active = true;

        public ContinuousGeneticAgent(IDna Dna) : base(Dna) { }

        public ContinuousGeneticAgent() : base() { }

        public virtual void Update() 
        {
            CalculateContinuousFitness();
            OnUpdate();
        }
        public virtual void OnUpdate() { }

        public virtual void OnKill() { }

        public bool IsActive() => Active;
        public void Kill()
        {
            CalculateCurrentFitness();
            Active = false;
            OnKill();
        }
        public virtual void CalculateContinuousFitness() { }
    }
}