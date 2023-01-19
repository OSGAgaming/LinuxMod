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
    public class MovableLiquid : ParticulateStep
    {
        public virtual int DissapationFactor { get; } = 5;

        public override void Step(Particulate p, ParticulateField particles)
        {
            byte[,] field = particles.Particulates;

            int randDir = (Main.rand.Next(0, 2) * 2 - 1) * DissapationFactor;

            int i = p.p.X;
            int j = p.p.Y;

            int randIDisp = Math.Min(field.GetLength(0) - 1, i + randDir);
            randIDisp = Math.Max(0, randIDisp);
            int randIDispInverse = Math.Max(0, i - randDir);
            randIDispInverse = Math.Min(field.GetLength(0) - 1, randIDispInverse);
            Point movedTo = new Point(i, j);

            int JRight = (int)MathHelper.Clamp(j + 1, 0, field.GetLength(1) - 1);
            int IRight = (int)MathHelper.Clamp(i + 1, 0, field.GetLength(0) - 1);
            int ILeft = (int)MathHelper.Clamp(i - 1, 0, field.GetLength(0) - 1);

            //TODO: Kill Trivaxy

            if (field[i, JRight] == 0 && !particles.CheckIfActiveTile(i, JRight)) movedTo = new Point(i, JRight);
            else if (field[ILeft, JRight] == 0 && !particles.CheckIfActiveTile(ILeft, JRight)) movedTo = new Point(ILeft, JRight);
            else if (field[IRight, JRight] == 0 && !particles.CheckIfActiveTile(IRight, JRight)) movedTo = new Point(IRight, JRight);
            else if (field[randIDisp, j] == 0 && !particles.CheckIfActiveTile(randIDisp, j))
            {
                bool hitSurface = false;

                if (randDir > 0)
                {
                    for (int a = i; a <= randIDisp; a++)
                    {
                        if (field[a, j] == 1 || particles.CheckIfActiveTile(a, j))
                        {
                            hitSurface = true;
                            movedTo = new Point(a - 1, j);
                            break;
                        }
                    }
                }
                else
                {
                    for (int a = i; a >= randIDisp; a--)
                    {
                        if (field[a, j] == 1 || particles.CheckIfActiveTile(a, j))
                        {
                            hitSurface = true;
                            movedTo = new Point(a + 1, j);
                            break;
                        }
                    }

                }
                if (!hitSurface) movedTo = new Point(randIDisp, j);
            }
            else if (field[randIDispInverse, j] == 0 && !particles.CheckIfActiveTile(randIDispInverse, j))
            {
                bool hitSurface = false;

                if (randIDispInverse > 0)
                {
                    for (int a = i; a <= randIDispInverse; a++)
                    {
                        if (field[a, j] == 1 || particles.CheckIfActiveTile(a, j))
                        {
                            hitSurface = true;
                            movedTo = new Point(a - 1, j);
                            break;
                        }
                    }
                }
                else
                {
                    for (int a = i; a >= randIDispInverse; a--)
                    {
                        if (field[a, j] == 1 || particles.CheckIfActiveTile(a, j))
                        {
                            hitSurface = true;
                            movedTo = new Point(a + 1, j);
                            break;
                        }
                    }

                }
                if (!hitSurface) movedTo = new Point(randIDispInverse, j);
            }

            field[i, j] = 0;
            field[movedTo.X, movedTo.Y] = p.type;

            p.p = movedTo;
        }
    }
}