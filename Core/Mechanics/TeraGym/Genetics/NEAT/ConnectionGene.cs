using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics.TeraGym.NEAT
{
    public class ConnectionGene : Gene
    {
        public NodeGene from;
        public NodeGene to;

        public double weight;
        public bool enabled = true;

        public ConnectionGene(NodeGene from, NodeGene to) : base(0)
        {
            this.from = from;
            this.to = to;
        }

        public override bool Equals(object o)
        {
            if (!(o is ConnectionGene)) return false;
            ConnectionGene c = (ConnectionGene)o;
            return from.Equals(c.from) && to.Equals(c.to);
        }

        public override int GetHashCode()
        {
            return from.GetHashCode() * NeatHost.MAX_NODES +  to.GetHashCode();
        }
    }
}