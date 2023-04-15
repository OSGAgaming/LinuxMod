using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics.TeraGym.NEAT
{
    public class Connection
    {
        public NeatNode from;
        public NeatNode to;

        public double weight;
        public bool enabled = true;

        public Connection(NeatNode from, NeatNode to)
        {
            this.from = from;
            this.to = to;
        }

    }
}