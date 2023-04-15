using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics.TeraGym.NEAT
{
    public class Gene
    {
        protected int innovationNumber;

        public Gene(int innovationNumber) => this.innovationNumber = innovationNumber;

        public int getInovationNumber() => innovationNumber;

        public void setInovationNumber(int n) => innovationNumber = n;
    }
}