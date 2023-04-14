using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;

namespace LinuxMod.Core.Mechanics
{
    public class Node
    {
        public float value;
        public float bias;

        public float[] weights;
    }
}