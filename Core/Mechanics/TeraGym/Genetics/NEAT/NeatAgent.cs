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
using LinuxMod.Core.Mechanics.TeraGym.NEAT;

namespace LinuxMod.Core.Mechanics
{
    public class NeatAgent : ContinuousGeneticAgent
    {
        private Species species;

        public void SetSpecies(Species species) => this.species = species;
        
        public Species GetSpecies() => species;

        public Genome GetGenome() => Dna as Genome;

        public void SetGenome(Genome g) => Dna = g;

        public double Distance(NeatAgent other) => GetGenome().Distance(other.GetGenome());

        public void Mutate() => GetGenome().Mutate();

        public void GenerateCalculator() => GetGenome().GenerateCalculator();

        public virtual void Reset() { }
    }
}