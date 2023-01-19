
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
    public class MovableSolid : ParticulateStep
    {
        public override void Step(Particulate p, ParticulateField particles)
        {
            byte[,] field = particles.Particulates;

            int randDir = (Main.rand.Next(0, 2) * 2 - 1);

            int i = p.p.X;
            int j = p.p.Y;

            int randIDisp = Math.Min(field.GetLength(0) - 1, i + randDir);
            randIDisp = Math.Max(0, randIDisp);
            int randIDispInverse = Math.Max(0, i - randDir);
            randIDispInverse = Math.Min(field.GetLength(0) - 1, randIDispInverse);

            Point movedTo = new Point(i, j);

            int JBottom = (int)MathHelper.Clamp(j + 1, 0, field.GetLength(1) - 1);
            int ILeft = (int)MathHelper.Clamp(randIDisp, 0, field.GetLength(0) - 1);
            int IRight = (int)MathHelper.Clamp(randIDispInverse, 0, field.GetLength(0) + 1);

            if (field[i, JBottom] == 0 && !particles.CheckIfActiveTile(i, JBottom)) movedTo = new Point(i, JBottom);
            else if (field[ILeft, JBottom] == 0 && !particles.CheckIfActiveTile(ILeft, JBottom)) movedTo = new Point(ILeft, JBottom);
            else if (field[IRight, JBottom] == 0 && !particles.CheckIfActiveTile(IRight, JBottom)) movedTo = new Point(IRight, JBottom);

            field[i, j] = 0;
            field[movedTo.X, movedTo.Y] = p.type;

            p.p = movedTo;
        }
    }
}