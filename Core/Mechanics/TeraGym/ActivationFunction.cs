using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System;

namespace LinuxMod.Core.Mechanics
{
    public abstract class ActivationFunction
    {
        public virtual float Compute(float x)
        {
            return x;
        }
    }

    public class LinearActivationFunction : ActivationFunction { }

    public class SigmoidActivationFunction : ActivationFunction
    {
        public override float Compute(float x)
        {
            return 1f / (float)(1 + Math.Pow(MathHelper.E, -x));
        }
    }

}