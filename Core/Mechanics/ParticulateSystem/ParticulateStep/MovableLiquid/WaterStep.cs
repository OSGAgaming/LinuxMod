using LinuxMod.Core.Helper.Extensions;
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
    public class WaterStep : MovableLiquid
    {
        public override void Draw(SpriteBatch sb, Particulate p, ParticulateField particles)
        {
            byte[,] field = particles.Particulates;
            if (field[p.p.X, p.p.Y - 1] == 0)
                particles.DrawAt(sb, p.p.X, p.p.Y, Color.AliceBlue * 0.5f);
            else
                particles.DrawAt(sb, p.p.X, p.p.Y, Color.CadetBlue * 0.5f);
        }
    }
}