
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
    public class Particulate
    {
        public Point p;
        public byte type;
        public Color c;
        public Vector2 velocity;

        public Particulate(Point p, byte type, ParticulateField field)
        {
            this.p = p;
            this.type = type;

            if (ParticulateStep.actions.ContainsKey(type))
                ParticulateStep.actions[type].InitialStep(this, field);
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

        public void TryMoveTo(int i, int j, ParticulateField field)
        {

        }
    }

    public class ParticulateField
    {
        public int RefreshSpeed => 1;

        public int ScrollCoolDown { get; set; }

        public static byte CurrentType { get; set; } = 1;
        public static int IterationSeed { get; set; } = 0;

        public const int Resolution = 2;

        public byte[,] Particulates;
        public List<Particulate> Particles = new List<Particulate>();
        public Vector2 Position;
        public Point Size;
        public int Life;

        public ParticulateField(Vector2 Position, int sizeX, int sizeY)
        {
            this.Position = Position - new Vector2(sizeX, sizeY);
            Particulates = new byte[sizeX, sizeY];
            Size = new Point(sizeX, sizeY);
        }

        public void DrawAt(SpriteBatch sb, int i, int j, Color c)
            => sb.Draw(Main.magicPixel, (Position + new Vector2(i, j) * Resolution).ForDraw(), new Rectangle(0, 0, Resolution, Resolution), c);

        public void Draw(SpriteBatch sb)
        {
            foreach (Particulate p in Particles)
            {
                p.Draw(sb, this);
            }
        }

        public void AddParticulate(int i, int j, byte type = 1)
        {
            if (i < 0 || j < 0 ||
                i > Particulates.GetLength(0) - 1 ||
                j > Particulates.GetLength(1) - 1) return;
            if (Particulates[i, j] != 0) return;
            if (CheckIfActiveTile(i, j)) return;

            Particulates[i, j] = type;
            Particles.Add(new Particulate(new Point(i, j), type, this));
        }

        public Point CheckTilePos(int i, int j)
        {
            Vector2 position = new Vector2(Position.X + i * Resolution, Position.Y + j * Resolution);
            return position.ToTileCoordinates();
        }

        public bool CheckIfActiveTile(int i, int j)
        {
            Tile t = Framing.GetTileSafely(CheckTilePos(i,j));

            return t.active() && Main.tileSolid[t.type] && t.slope() == 0 
                && !t.bottomSlope()
                && !t.rightSlope()
                && !t.leftSlope()
                && !t.topSlope();
        }

        public void Update()
        {
            Life++;

            Debuging();

            if (Life % RefreshSpeed == 0)
            {
                IterationSeed = Main.rand.Next(1000);
                foreach (Particulate p in Particles)
                {
                    p.Step(this);
                }
            }
        }

        public void Debuging()
        {
            if (ScrollCoolDown > 0) ScrollCoolDown--;

            if (Main.LocalPlayer.controlUp && ScrollCoolDown == 0)
            {
                CurrentType++;
                if (CurrentType > 5) CurrentType = 1;
                ScrollCoolDown = 30;

                Main.NewText("Now spewing type: " + CurrentType);
            }

            if (Main.LocalPlayer.controlUseItem)
            {
                for(int i = -2; i < 3; i++)
                {
                    for (int j = -2; j < 3; j++)
                    {
                        Vector2 mousePos = (Main.MouseWorld - Position) / Resolution;
                        AddParticulate((int)mousePos.X + i, (int)mousePos.Y + j, CurrentType);
                    }
                }
            }
        }
    }
}