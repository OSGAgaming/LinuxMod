using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics.TeraGym.NEAT
{
    public class NodeGene : Gene
    {
        public double x, y;

        public NodeGene(int innovationNumber) : base(innovationNumber) { }

        public bool equals(object o)
        {
            if(!(o is NodeGene)) return false;
            return innovationNumber == ((NodeGene)o).innovationNumber; 
        }
        public int hashCode() => innovationNumber;
    }
}