
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
    public class ParticulateStep
    {
        public static Dictionary<int, ParticulateStep> actions = new Dictionary<int, ParticulateStep>();

        public static void Load()
        {
            LoadStepToID<SandStep>(2);
            LoadStepToID<WaterStep>(3);
            LoadStepToID<SandStep>(4);
            LoadStepToID<GasStep>(5);
        }

        public static void Unload() => actions.Clear();


        public static void LoadStepToID<T>(int ID) where T : ParticulateStep, new() => actions.Add(ID, new T());


        public virtual void Step(Particulate p, ParticulateField particles) { }

        public virtual void Draw(SpriteBatch sb, Particulate p, ParticulateField particles) { }
    }

    public class WaterStep : ParticulateStep
    {
        public override void Draw(SpriteBatch sb, Particulate p, ParticulateField particles)
        {
            byte[,] field = particles.Particulates;
            if (field[p.p.X, p.p.Y - 1] == 0)
                particles.DrawAt(sb, p.p.X, p.p.Y, Color.AliceBlue * 0.5f);
            else
                particles.DrawAt(sb, p.p.X, p.p.Y, Color.CadetBlue * 0.5f);
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

            int JRight = (int)MathHelper.Clamp(j + 1, 0, field.GetLength(1) - 1);
            int IRight = (int)MathHelper.Clamp(i + 1, 0, field.GetLength(0) + 1);
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
            int IRight = (int)MathHelper.Clamp(i + 1, 0, field.GetLength(0) + 1);
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
    public class SandStep : ParticulateStep
    {
        public override void Draw(SpriteBatch sb, Particulate p, ParticulateField particles)
        {
            byte[,] field = particles.Particulates;

            if (field[p.p.X, p.p.Y - 1] == 0)
                particles.DrawAt(sb, p.p.X, p.p.Y, new Color(58, 49, 19, 255));
            else
                particles.DrawAt(sb, p.p.X, p.p.Y, p.c);
        }

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
            int ILeft = (int)MathHelper.Clamp(i + randDir, 0, field.GetLength(0) - 1);
            int IRight = (int)MathHelper.Clamp(i - randDir, 0, field.GetLength(0) + 1);

            if (field[i, JBottom] == 0 && !particles.CheckIfActiveTile(i, JBottom)) movedTo = new Point(i, JBottom);
            else if (field[ILeft, JBottom] == 0 && !particles.CheckIfActiveTile(ILeft, JBottom)) movedTo = new Point(ILeft, JBottom);
            else if (field[IRight, JBottom] == 0 && !particles.CheckIfActiveTile(IRight, JBottom)) movedTo = new Point(IRight, JBottom);

            field[i, j] = 0;
            field[movedTo.X, movedTo.Y] = p.type;

            p.p = movedTo;
        }
    }

    public class Particulate
    {
        public Point p;
        public byte type;
        public Color c;
        public Particulate(Point p, byte type, Color c)
        {
            this.p = p;
            this.type = type;
            this.c = c;
        }

        public virtual void Step(ParticulateField field)
        {
            if (ParticulateStep.actions.ContainsKey(type))
                ParticulateStep.actions[type].Step(this, field);
        }

        public void Draw(SpriteBatch sb, ParticulateField field)
        {
            if (ParticulateStep.actions.ContainsKey(type))
                ParticulateStep.actions[type].Draw(sb, this, field);
        }
    }

    public class ParticulateField
    {
        public byte[,] Particulates;
        public List<Particulate> Particles = new List<Particulate>();

        public Vector2 Position;

        public int RefreshSpeed => 1;
        public int Life;
        public const int Resolution = 2;

        public Point Size;
        
        public ParticulateField(Vector2 Position, int sizeX, int sizeY)
        {
            this.Position = Position - new Vector2(sizeX, sizeY);
            Particulates = new byte[sizeX, sizeY];

            for (int i = 0; i < Particulates.GetLength(0); i++)
            {
                AddParticulate(i, sizeY - 1, Color.Black, 1);
            }

            for (int i = 0; i < Particulates.GetLength(1); i++)
            {
                AddParticulate(0, i, Color.Black, 1);
            }

            for (int i = 0; i < Particulates.GetLength(1); i++)
            {
                AddParticulate(sizeX - 1, i, Color.Black, 1);
            }

            Size = new Point(sizeX, sizeY);
        }

        public void DrawAt(SpriteBatch sb, int i, int j, Color c)
            => 
            sb.Draw(Main.magicPixel, (Position + new Vector2(i, j) * Resolution).ForDraw(), new Rectangle(0, 0, Resolution, Resolution), c);

        public void Draw(SpriteBatch sb)
        {
            foreach(Particulate p in Particles)
            {
                p.Draw(sb, this);
            }
        }

        public void AddParticulate(int i, int j, Color c, byte type = 1)
        {
            if (Particulates[i, j] != 0) return;
            if (CheckIfActiveTile(i, j)) return;

            Particulates[i, j] = type;
            Particles.Add(new Particulate(new Point(i, j), type, c));
        }

        public bool CheckIfActiveTile(int i, int j)
        {
            Vector2 position = new Vector2(Position.X + i * Resolution, Position.Y + j * Resolution);
            Point p = position.ToTileCoordinates();
            Tile t = Framing.GetTileSafely(p);

            return t.active() && Main.tileSolid[t.type];
        }

        public void CreateWater()
        {
            Color c = Color.White;

            for (int a = -2; a < 3; a++)
            {
                for (int b = -2; b < 3; b++)
                {
                    AddParticulate(Size.X / 2 + a + (int)(Math.Sin(Life / 20f) * 20), Size.Y / 2 + b + (int)(Math.Cos(Life / 20f) * 4), c, 3);
                }
            }

        }

        public void CreateSand()
        {
            int rand = Main.rand.Next(0, 4);
            Color c = Color.White;

            if (rand == 0) c = new Color(223, 219, 147);
            else if (rand == 1) c = new Color(212, 192, 100);
            else if (rand == 2) c = new Color(186, 168, 84);
            else if (rand == 3) c = new Color(139, 131, 59);

            for (int a = -2; a < 3; a++)
            {
                for (int b = -2; b < 3; b++)
                {
                    AddParticulate(Size.X / 2 + a + (int)(Math.Sin(Life / 20f) * 20), Size.Y / 2 + b + (int)(Math.Cos(Life / 20f) * 4), c, 2);
                }
            }

        }

        public void CreateCoins()
        {
            int rand = Main.rand.Next(0, 4);
            Color c = Color.White;

            if (rand == 0) c = new Color(240, 183, 160);
            else if (rand == 1) c = new Color(226, 118, 76);
            else if (rand == 2) c = new Color(183, 88, 25);
            else if (rand == 3) c = new Color(150, 67, 22);
            else if (rand == 4) c = new Color(122, 55, 18);
            else if (rand == 5) c = new Color(85, 30, 0);

            for (int a = -2; a < 3; a++)
            {
                for (int b = -2; b < 3; b++)
                {
                    AddParticulate(Size.X / 2 + a + (int)(Math.Sin(Life / 20f) * 20), Size.Y / 2 + b + (int)(Math.Cos(Life / 20f) * 4), c, 4);
                }
            }

        }

        public void CreateGas()
        {
            Color c = Color.White;

            for (int a = -2; a < 3; a++)
            {
                for (int b = -2; b < 3; b++)
                {
                    AddParticulate(Size.X / 2 + a + (int)(Math.Sin(Life / 20f + a) * 5), Size.Y / 2 + b + (int)(Math.Cos(Life / 20f + b) * 5), c, 5);
                }
            }

        }

        public void Update()
        {
            Life++;

            if (Life % RefreshSpeed == 0)
            {
                CreateGas();

                foreach (Particulate p in Particles)
                {
                    p.Step(this);
                }
            }
        }
    }
}