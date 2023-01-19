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
    public class CoinStep : MovableSolid
    {
        public override void Draw(SpriteBatch sb, Particulate p, ParticulateField particles)
        {
            byte[,] field = particles.Particulates;

            if (field[p.p.X, p.p.Y - 1] == 0)
                particles.DrawAt(sb, p.p.X, p.p.Y, new Color(58, 49, 19, 255));
            else
                particles.DrawAt(sb, p.p.X, p.p.Y, p.c);
        }

        public override void InitialStep(Particulate p, ParticulateField particles)
        {
            int rand = particles.Life * 17 % 6;
            Color c = Color.White;

            if (rand == 0) c = new Color(240, 183, 160);
            else if (rand == 1) c = new Color(226, 118, 76);
            else if (rand == 2) c = new Color(183, 88, 25);
            else if (rand == 3) c = new Color(150, 67, 22);
            else if (rand == 4) c = new Color(122, 55, 18);
            else if (rand == 5) c = new Color(85, 30, 0);

            p.c = c;
        }
    }
}