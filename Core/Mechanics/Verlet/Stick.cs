using EEMod.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core.Mechanics.Verlet
{
    public class Stick
    {
        public Texture2D tex;
        public int[] StickPoints;
        public float Length;

        public Stick(int[] points, int Length, Texture2D texture)
        {
            StickPoints = points;
            tex = texture;

            this.Length = Length;
        }
    }
}
