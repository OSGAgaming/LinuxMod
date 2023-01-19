
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
    public class ImmovableSolid : ParticulateStep
    {
        public override void Draw(SpriteBatch sb, Particulate p, ParticulateField particles)
        {
            Point tile = particles.CheckTilePos(p.p.X, p.p.Y);
            Color light = Lighting.GetColor(tile.X, tile.Y);

            particles.DrawAt(sb, p.p.X, p.p.Y, p.c.MultiplyRGB(light));
        }

        public override void InitialStep(Particulate p, ParticulateField particles)
        {
            int rand = ParticulateField.IterationSeed % 4;
            Color c = Color.White;

            if (rand == 0) c = new Color(0, 0, 0);
            else if (rand == 1) c = new Color(20, 20, 20);
            else if (rand == 2) c = new Color(30, 30, 30);
            else if (rand == 3) c = new Color(35, 35, 35);

            p.c = c;
        }
    }
}