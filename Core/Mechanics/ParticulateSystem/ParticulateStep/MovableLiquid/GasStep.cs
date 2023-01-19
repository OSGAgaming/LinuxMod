
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
    public class GasStep : ParticulateStep
    {
        public override void Draw(SpriteBatch sb, Particulate p, ParticulateField particles)
        {
            byte[,] field = particles.Particulates;
            if (p.p.Y > 0 && field[p.p.X, p.p.Y - 1] == 0)
                particles.DrawAt(sb, p.p.X, p.p.Y, Color.Gray * 0.5f);
            else
                particles.DrawAt(sb, p.p.X, p.p.Y, Color.WhiteSmoke * 0.5f);
        }

        public override void Step(Particulate p, ParticulateField particles)
        {
            byte[,] field = particles.Particulates;

            int randDir = (Main.rand.Next(0, 2) * 2 - 1) * 5;

            int i = p.p.X;
            int j = p.p.Y;

            int randIDisp = Math.Min(field.GetLength(0) - 1, i + randDir);
            randIDisp = Math.Max(0, randIDisp);
            int randIDispInverse = Math.Max(0, i - randDir);
            randIDispInverse = Math.Min(field.GetLength(0) - 1, randIDispInverse);
            Point movedTo = new Point(i, j);

            int JBottom = (int)MathHelper.Clamp(j - 1, 0, field.GetLength(1) - 1);
            int IRight = (int)MathHelper.Clamp(i + 1, 0, field.GetLength(0) - 1);
            int ILeft = (int)MathHelper.Clamp(i - 1, 0, field.GetLength(0) - 1);

            //TODO: Kill Trivaxy

            if (field[i, JBottom] == 0 && !particles.CheckIfActiveTile(i, JBottom)) movedTo = new Point(i, JBottom);
            else if (field[ILeft, JBottom] == 0 && !particles.CheckIfActiveTile(ILeft, JBottom)) movedTo = new Point(ILeft, JBottom);
            else if (field[IRight, JBottom] == 0 && !particles.CheckIfActiveTile(IRight, JBottom)) movedTo = new Point(IRight, JBottom);
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
                        if (field[a, j] == 3 || particles.CheckIfActiveTile(a, j))
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