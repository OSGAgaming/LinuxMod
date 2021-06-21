using EEMod.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core.Mechanics.Verlet
{
    public static class VerletHelpers
    {
        public static int[] CreateVerletSquare(VerletSystem system, Vector2 pos, int size)
        {
            int a = system.CreateVerletPoint(pos + new Vector2(-size / 2, -size / 2));
            int b = system.CreateVerletPoint(pos + new Vector2(size / 2, -size / 2));
            int c = system.CreateVerletPoint(pos + new Vector2(size / 2, size / 2));
            int d = system.CreateVerletPoint(pos + new Vector2(-size / 2, size / 2));

            system.BindPoints(a, b);
            system.BindPoints(b, c);
            system.BindPoints(c, d);
            system.BindPoints(d, a);
            system.BindPoints(a, c);

            return new int[] { a, b, c, d };
        }
    }
}
