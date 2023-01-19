
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core
{
    public class SandStep : MovableSolid
    {
        public override void Draw(SpriteBatch sb, Particulate p, ParticulateField particles)
        {
            byte[,] field = particles.Particulates;
            Point tile = particles.CheckTilePos(p.p.X, p.p.Y);

            Color light = Lighting.GetColor(tile.X, tile.Y);

            if (p.p.Y > 0 && field[p.p.X, p.p.Y - 1] == 0)
                particles.DrawAt(sb, p.p.X, p.p.Y, new Color(58, 49, 19, 255).MultiplyRGB(light));
            else
                particles.DrawAt(sb, p.p.X, p.p.Y, p.c.MultiplyRGB(light));
        }

        public override void InitialStep(Particulate p, ParticulateField particles)
        {
            int rand = ParticulateField.IterationSeed % 4;
            Color c = Color.White;

            if (rand == 0) c = new Color(223, 219, 147);
            else if (rand == 1) c = new Color(212, 192, 100);
            else if (rand == 2) c = new Color(186, 168, 84);
            else if (rand == 3) c = new Color(139, 131, 59);

            p.c = c;
        }
    }
}