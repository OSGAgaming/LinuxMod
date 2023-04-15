using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics.TeraGym.NEAT
{
    public class NeatNode : IComparable<NeatNode>
    {
        public float x;
        public float value;
        public List<Connection> connections = new List<Connection>();
        public ActivationFunction activation;

        public NeatNode(float x, ActivationFunction activation = null)
        {
            this.x = x;
            this.activation = activation;
            if (activation == null) this.activation = new SigmoidActivationFunction();
        }

        public void Calculate()
        {
            double s = 0;
            foreach(Connection c in connections)
            {
                if (c.enabled) s += c.weight * c.from.value;
            }
            value = activation?.Compute((float)s) ?? 0;
        }

        public int CompareTo(NeatNode o)
        {
            if (x > o.x) return -1;
            if (x < o.x) return 1;
            return 0;
        }
    }
}