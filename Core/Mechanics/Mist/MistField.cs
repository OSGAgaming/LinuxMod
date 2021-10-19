using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class MistField : IComponent, IDisposable
    {
        float[] DensityTable;
        float[] PreviousDensityTable;

        float[] VectorFieldX;
        float[] PreviousVectorFieldX;

        float[] VectorFieldY;
        float[] PreviousVectorFieldY;

        private float DiffusionRate => 0f;
        private float Viscocity => 0f;
        private float dT => 1;
        public int Lifetime => 1200;
        private float EaseOutFactor => 0f;


        private readonly int iter = 4;

        public int SimulationSize { get; set; } = 100;
        private int CellSize { get; set; } = 16;
        private Vector2 GlobalSpace { get; set; }
        public int TimeAlive { get; set; }

        private int N => SimulationSize;

        public MistField(int SimulationSize, int CellSize, Vector2 GlobalSpace)
        {
            DensityTable = new float[N * N];
            PreviousDensityTable = new float[N * N];
            VectorFieldX = new float[N * N];
            PreviousVectorFieldX = new float[N * N];
            VectorFieldY = new float[N * N];
            PreviousVectorFieldY = new float[N * N];

            this.SimulationSize = SimulationSize;
            this.CellSize = CellSize;
            this.GlobalSpace = new Vector2((int)(GlobalSpace.X / CellSize) * CellSize, (int)(GlobalSpace.Y / CellSize) * CellSize);
        }

        public void Update()
        {
            TimeAlive++;

            //the velocity strangely enough diffuses and moves just like the density
            ResolveVelocity();
            ResolveDensity();

            //Create pressure at the center of field
            int cx = (int)(0.5f * N);
            int cy = (int)(0.5f * N);
            for (int i = 0; i < 10; i++)
            {
                DensityTable[XY(cx, cy)] += Main.rand.NextFloat(100f,500f);
            }

            //Apply a rotating vector field to the center of field
            float angle = Main.GameUpdateCount * 0.01f + 1.57f;
            Vector2 v = angle.ToRotationVector2();
            v *= 0.1f;
            VectorFieldX[XY(cx,cy)] += v.X;
            VectorFieldY[XY(cx, cy)] += v.Y;

            dEaseOut(EaseOutFactor);
        }
        public int XY(int X, int Y) => X + Y * N;

        public void ConfigureOcclusion(int type, ref float[] array)
        {
            // a bunch of boundary cases, there is unfortunately no clever way of doing this for all permutations afaik
            //stevie will probably figure it out

            for (int i = 1; i < N - 1; i++)
            {
                array[XY(0, i)] = type == 1 ? -array[XY(1, i)] : array[XY(1, i)];
                array[XY(N - 1, i)] = type == 1 ? -array[XY(N - 2, i)] : array[XY(N - 2, i)];
                array[XY(i, 0)] = type == 2 ? -array[XY(i, 1)] : array[XY(i, 1)];
                array[XY(i, N - 1)] = type == 2 ? -array[XY(i, N - 2)] : array[XY(i, N - 2)];
            }

            array[XY(0, 0)] = 0.5f * (array[XY(1, 0)] + array[XY(0, 1)]);
            array[XY(0, N - 1)] = 0.5f * (array[XY(1, N - 1)] + array[XY(0, N - 2)]);
            array[XY(N - 1, 0)] = 0.5f * (array[XY(N - 2, 0)] + array[XY(N - 1, 1)]);
            array[XY(N - 1, N - 1)] = 0.5f * (array[XY(N - 2, N - 1)] + array[XY(N - 1, N - 2)]);

            //for custom boundaries, in this case tiles
            for (int i = 1; i < N - 1; i++)
            {
                for (int j = 1; j < N - 1; j++)
                {
                    Vector2 pU = GlobalSpace + new Vector2(i, j - 1) * CellSize;
                    Vector2 pD = GlobalSpace + new Vector2(i, j + 1) * CellSize;
                    Vector2 pL = GlobalSpace + new Vector2(i - 1, j) * CellSize;
                    Vector2 pR = GlobalSpace + new Vector2(i + 1, j) * CellSize;

                    Vector2 p = GlobalSpace + new Vector2(i, j) * CellSize;

                    Tile tileU = Framing.GetTileSafely(Utils.ToTileCoordinates(pU));
                    Tile tileD = Framing.GetTileSafely(Utils.ToTileCoordinates(pD));
                    Tile tileL = Framing.GetTileSafely(Utils.ToTileCoordinates(pL));
                    Tile tileR = Framing.GetTileSafely(Utils.ToTileCoordinates(pR));
                    Tile tile = Framing.GetTileSafely(Utils.ToTileCoordinates(p));             

                    bool u = tileU.active() && Main.tileSolid[tileU.type];
                    bool d = tileD.active() && Main.tileSolid[tileD.type];
                    bool l = tileL.active() && Main.tileSolid[tileL.type];
                    bool r = tileR.active() && Main.tileSolid[tileR.type];

                    if (tile.active() && Main.tileSolid[tile.type] && u && d && l && r)
                    {
                        array[XY(i, j)] = 0;
                        continue;
                    }
                    if (u && r && !d && !l) array[XY(i, j)] = 0.5f * (array[XY(i - 1, j)] + array[XY(i, j + 1)]);
                    if (!u && !r && d && l) array[XY(i, j)] = 0.5f * (array[XY(i + 1, j)] + array[XY(i, j - 1)]);
                    if (u && !r && !d && l) array[XY(i, j)] = 0.5f * (array[XY(i + 1, j)] + array[XY(i, j + 1)]);
                    if (!u && r && d && !l) array[XY(i, j)] = 0.5f * (array[XY(i - 1, j)] + array[XY(i, j - 1)]);

                    if (u && !r && !d && !l) array[XY(i, j)] = type == 2 ? -array[XY(i, j + 1)] : array[XY(i, j + 1)];
                    if (d && !u && !r && !l) array[XY(i, j)] = type == 2 ? -array[XY(i, j - 1)] : array[XY(i, j - 1)];
                    if (l && !r && !d && !u) array[XY(i, j)] = type == 1 ? -array[XY(i + 1, j)] : array[XY(i + 1, j)];
                    if (r && !u && !d && !l) array[XY(i, j)] = type == 1 ? -array[XY(i - 1, j)] : array[XY(i - 1, j)];

                    if (!u && r && d && l) array[XY(i, j)] = type == 2 ? -array[XY(i, j - 1)] : array[XY(i, j - 1)];
                    if (!d && u && r && l) array[XY(i, j)] = type == 2 ? -array[XY(i, j + 1)] : array[XY(i, j + 1)];
                    if (!l && r && d && u) array[XY(i, j)] = type == 1 ? -array[XY(i - 1, j)] : array[XY(i - 1, j)];
                    if (!r && u && d && l) array[XY(i, j)] = type == 1 ? -array[XY(i + 1, j)] : array[XY(i + 1, j)];
                }
            }
        }

        void Project(ref float[] u, ref float[] v, ref float[] p, ref float[] div)
        {
            //use poisson equations to make sure vector field magnitudes are being conserved
            //if you want to learn more about this, I suggest starting at swirl and gradient fields.

            int i, j, k;
            float h;
            h = 1.0f / N;

            for (i = 1; i < N - 1; i++)
            {
                for (j = 1; j < N - 1; j++)
                {
                    div[XY(i, j)] = -0.5f * h * (u[XY(i + 1, j)] - u[XY(i - 1, j)] +
                      v[XY(i, j + 1)] - v[XY(i, j - 1)]);
                    p[XY(i, j)] = 0;
                }
            }
            ConfigureOcclusion(0, ref div);
            ConfigureOcclusion(0, ref p);

            for (k = 0; k < iter; k++)
            {
                for (i = 1; i < N - 1; i++)
                {
                    for (j = 1; j < N - 1; j++)
                    {
                        p[XY(i, j)] = (div[XY(i, j)] + p[XY(i - 1, j)] + p[XY(i + 1, j)] +
                          p[XY(i, j - 1)] + p[XY(i, j + 1)]) / 6f;
                    }
                }
                ConfigureOcclusion(0,ref p);
            }
            for (i = 1; i < N - 1; i++)
            {
                for (j = 1; j < N - 1; j++)
                {
                    u[XY(i, j)] -= 0.5f * (p[XY(i + 1, j)] - p[XY(i - 1, j)]) / h;
                    v[XY(i, j)] -= 0.5f * (p[XY(i, j + 1)] - p[XY(i, j - 1)]) / h;
                }
            }
            ConfigureOcclusion(1, ref u);
            ConfigureOcclusion(2, ref v);
        }

        public void ResolveDensity()
        {
            //Diffuse
            Iterate(0, ref PreviousDensityTable, DensityTable, DiffusionRate);
            //Apply vector field
            AdVec(0, ref DensityTable, PreviousDensityTable, VectorFieldX, VectorFieldY);
        }

        public void ResolveVelocity()
        {
            //Diffuse in both directions
            Iterate(1, ref PreviousVectorFieldX, VectorFieldX, Viscocity);
            Iterate(2, ref PreviousVectorFieldY, VectorFieldY, Viscocity);

            //see top of project
            Project(ref PreviousVectorFieldX, ref PreviousVectorFieldY, ref VectorFieldX, ref VectorFieldY);

            //Vector field moves along itself
            AdVec(1, ref VectorFieldX, PreviousVectorFieldX, PreviousVectorFieldX, PreviousVectorFieldY);
            AdVec(2, ref VectorFieldY, PreviousVectorFieldY, PreviousVectorFieldX, PreviousVectorFieldY);

            Project(ref VectorFieldX, ref VectorFieldY, ref PreviousVectorFieldX, ref PreviousVectorFieldY);
        }

        public void Iterate(int type, ref float[] a, float[] _a, float diff)
        {
            //Diffusion step
            float visc = dT * diff * (N - 2) * (N - 2);

            //Takes average of 4 squares around it (0s)
            //X 0 X
            //0 0 0
            //X 0 X
            //and gradually converges to the average of those squares

            //target = average of surrouding cells
            //n : new
            //c : current
            //k = diffusion rate

            //Value of diffusion greater than 1, will go crazy if you do a simple linear convergence [nX = cX + (target - cX) * k]
            //We need to work backwards
            //Get [cX = nX - (target - nX) * k] 
            //Rearange to to get nX on the other side again to get
            //cX = nX - target * k + nX * k => nX = (cX + target * k) / (1 + k * (constant))                      constant is just to affect the converge speed.
            //we dont have target :P, so let target be anything (I just used the ones we currently have)
            

            for (int k = 0; k < iter; k++)
            {
                for (int i = 1; i < N - 1; i++)
                {
                    for (int j = 1; j < N - 1; j++)
                    {
                        float pressure = _a[XY(i, j)];

                        float totalPressure;
                        float numberOfNeighbours;

                        totalPressure = a[XY(i + 1, j)] + a[XY(i - 1, j)] + a[XY(i, j + 1)] + a[XY(i, j - 1)];
                        numberOfNeighbours = 6;

                        a[XY(i, j)] = (pressure + totalPressure * visc) / (1 + numberOfNeighbours * visc);
                    }
                }
                ConfigureOcclusion(type, ref a);
            }
        }

        public void dEaseOut(float easeout)
        {
            for (int i = 0; i < DensityTable.Length; i++)
            {
                float d = DensityTable[i];
                DensityTable[i] = MathHelper.Clamp(d - easeout, 0, 255);
            }
        }

        public void AdVec(int type, ref float[] a, float[] _a, float[] u, float[] v)
        {
            //this handles how density/velocity is moved across the field
            //sometimes the target cell is fractional,
            //so to represent it, we get its position inside the cell,
            //and then linearly interpolate neighbouring cells density/velocity based on this position.
            //3 egs,
            //-------------------------------------------------------------------------------------------
            //if the movement ends up top left of a cell, then only that cell counts
            //-------------------------------------------------------------------------------------------
            //if it ends up in the middle of a cell, then the new value is an equal combination between
            //that cell, the cell to the right, the cell down, and the cell to the bottom right
            //-------------------------------------------------------------------------------------------
            //if it ends more to the bottom right of the cell, it will represent the bottom right cell the most
            //the cells down and right second most, and the top left cell the least

            //the exact interpolation defined spefically below

            float visc = dT * (N - 2);
            float ifloat, jfloat;
            int i, j;
            int XGrid, YGrid, XGrid1, YGrid1;

            for (j = 1, jfloat = 1; j < N - 1; j++, jfloat++)
            {
                for (i = 1, ifloat = 1; i < N - 1; i++, ifloat++)
                {
                    float X = ifloat - u[XY(i, j)] * visc;
                    float Y = jfloat - v[XY(i, j)] * visc;

                    X = MathHelper.Clamp(X, 0.5f, N - 1.5f);

                    Y = MathHelper.Clamp(Y, 0.5f, N - 1.5f);

                    XGrid = (int)Math.Floor(X);
                    XGrid1 = XGrid + 1;
                    YGrid = (int)Math.Floor(Y);
                    YGrid1 = YGrid + 1;

                    float XRelative1 = X - XGrid;
                    float XRelative0 = 1 - XRelative1;
                    float YRelative1 = Y - YGrid;
                    float YRelative0 = 1 - YRelative1;

                    a[XY(i, j)] =
                      XRelative0 * (YRelative0 * _a[XY(XGrid, YGrid)] + YRelative1 * _a[XY(XGrid, YGrid1)]) +
                      XRelative1 * (YRelative0 * _a[XY(XGrid1, YGrid)] + YRelative1 * _a[XY(XGrid1, YGrid1)]);
                }
            }

            //we cant let velocity escape, if not we can have gas currents on one side
            //of a boundary affect the currents in the other side
            ConfigureOcclusion(type, ref a);
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < SimulationSize; i++)
            {
                for (int j = 0; j < SimulationSize; j++)
                {
                    float mistParticle = DensityTable[XY(i, j)];
                    Vector2 position = GlobalSpace + new Vector2(i, j) * CellSize;
                    Vector2 drawPos = position.ForDraw();

                    float XPassAlpha = (float)Math.Sin(i * (Math.PI / SimulationSize));
                    float YPassAlpha = (float)Math.Sin(j * (Math.PI / SimulationSize));

                    //Keep for later
                    float BorderAlpha = Math.Max(0, (SimulationSize / 2 - Vector2.Distance(new Vector2(i, j), new Vector2(SimulationSize / 2))) / (SimulationSize / 2));
                    BorderAlpha = XPassAlpha * XPassAlpha * YPassAlpha * YPassAlpha;

                    sb.Draw(Main.magicPixel, new Rectangle((int)drawPos.X, (int)drawPos.Y, CellSize, CellSize), Color.DarkGray * (mistParticle/120f) * BorderAlpha);
                }
            }
        }

        public void Dispose()
        {
            DensityTable = null;
        }
    }
}


