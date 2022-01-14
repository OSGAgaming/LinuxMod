using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private float DiffusionRate => 0;
        private float Viscocity => 0;
        private float dT => .02f;
        public int Lifetime => 300;
        private float EaseOutFactor => 0.1f;

        private readonly int iter = 2;

        public int SimulationSize { get; set; }
        private int CellSize { get; set; }
        private Vector2 GlobalSpace { get; set; }
        public int TimeAlive { get; set; }

        private int N => SimulationSize;

        private RenderTarget2D DensityTarget;
        private RenderTarget2D PreviousDensityTarget;

        private RenderTarget2D VectorTargetX;
        private RenderTarget2D VectorTargetY;

        private RenderTarget2D PVectorTargetX;
        private RenderTarget2D PVectorTargetY;

        private RenderTarget2D BufferTarget { get; set; }

        private RenderTarget2D AddVecSourceBuffer;
        private RenderTarget2D AddDensitySourceBuffer;

        public MistField(int SimulationSize, int CellSize, Vector2 GlobalSpace)
        {
            this.SimulationSize = SimulationSize;

            BufferTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, CellSize * SimulationSize, CellSize * SimulationSize);

            DensityTarget = new RenderTarget2D(
                Main.graphics.GraphicsDevice, CellSize * SimulationSize, CellSize * SimulationSize, false,
                                           Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                           DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            AddVecSourceBuffer = new RenderTarget2D(Main.graphics.GraphicsDevice, CellSize * SimulationSize, CellSize * SimulationSize);

            AddDensitySourceBuffer = new RenderTarget2D(
                Main.graphics.GraphicsDevice, CellSize * SimulationSize, CellSize * SimulationSize, false,
                                           Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                           DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            Color[] c = new Color[DensityTarget.Width * DensityTarget.Height];
            Color[] cV = new Color[DensityTarget.Width * DensityTarget.Height];
            Color[] cV2 = new Color[DensityTarget.Width * DensityTarget.Height];

            for (int i = -10; i < 10; i++)
            {
                for (int j = -10; j < 10; j++)
                {
                    //c[c.Length / 2 + i + j * DensityTarget.Width + DensityTarget.Width / 2] = new Color(0, 1f, 0f);
                    cV[c.Length / 2 + i + j * DensityTarget.Width + DensityTarget.Width / 2] = new Color(0, 0, 0.5f);
                }
            }

            for (int i = 0; i < DensityTarget.Width; i++)
            {
                for (int j = 0; j < DensityTarget.Width; j++)
                {
                    //cV[i + j * DensityTarget.Width] = new Color(0, 1f, 0.5f);
                }
            }

            PreviousDensityTarget = new RenderTarget2D(
                Main.graphics.GraphicsDevice, CellSize * SimulationSize, CellSize * SimulationSize, false,
                                           Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                           DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            //DensityTarget.SetData(c);
            //PreviousDensityTarget.SetData(c);

            VectorTargetX = new RenderTarget2D(
                Main.graphics.GraphicsDevice, CellSize * SimulationSize, CellSize * SimulationSize, false,
                                           Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                           DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            VectorTargetY = new RenderTarget2D(
                Main.graphics.GraphicsDevice, CellSize * SimulationSize, CellSize * SimulationSize, false,
                                           Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                           DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            PVectorTargetX = new RenderTarget2D(
                Main.graphics.GraphicsDevice, CellSize * SimulationSize, CellSize * SimulationSize, false,
                                           Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                           DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            PVectorTargetY = new RenderTarget2D(
                Main.graphics.GraphicsDevice, CellSize * SimulationSize, CellSize * SimulationSize, false,
                                           Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                           DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            //PVectorTargetY.SetData(cV);
            //PVectorTargetX.SetData(cV);
            VectorTargetY.SetData(cV);
            VectorTargetX.SetData(cV);

            DensityTable = new float[N * N];
            PreviousDensityTable = new float[N * N];
            VectorFieldX = new float[N * N];
            PreviousVectorFieldX = new float[N * N];
            VectorFieldY = new float[N * N];
            PreviousVectorFieldY = new float[N * N];

            this.CellSize = CellSize;
            this.GlobalSpace = new Vector2((int)(GlobalSpace.X / CellSize) * CellSize, (int)(GlobalSpace.Y / CellSize) * CellSize);
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                TimeAlive++;

                //the velocity strangely enough diffuses and moves just like the density

                //Create pressure at the center of field
                int cx = (int)(0.5f * N);
                int cy = (int)(0.5f * N);

                float angle = TimeAlive * 0.05f;
                Vector2 v = angle.ToRotationVector2();
                v *= 1f;

                float signX = (Math.Sign(v.X) + 1) / 2f;
                float signY = (Math.Sign(v.Y) + 1) / 2f;

                float absX = Math.Abs(v.X);
                float absY = Math.Abs(v.Y);

                AdSource(DensityTarget, AddVecSourceBuffer, (sb) =>
                {
                    sb.Draw(Main.magicPixel, new Rectangle(N / 2, N / 2, 4, 4), new Color(0f, 1f, 0));
                });

                AdSource(VectorTargetX, AddVecSourceBuffer, (sb) =>
                {
                    sb.Draw(Main.magicPixel, new Rectangle(N / 2, N / 2, 8, 8), new Color((1 - signX) * absX, signX * absX, 0));
                });

                AdSource(VectorTargetY, AddVecSourceBuffer, (sb) =>
                {
                    sb.Draw(Main.magicPixel, new Rectangle(N / 2, N / 2, 8, 8), new Color((1 - signY) * absY, signY * absY, 0));
                });

                ResolveVelocity();
                ResolveDensity();
            }
        }
        public int XY(int X, int Y) => X + Y * N;

        public void CopyContents(RenderTarget2D a, RenderTarget2D b)
        {
            Main.graphics.GraphicsDevice.SetRenderTarget(a);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(b, b.Bounds, Color.White);

            Main.spriteBatch.End();
        }

        public void DrawToTargetWithBuffer(RenderTarget2D t, Action<SpriteBatch> Draw)
        {
            Main.graphics.GraphicsDevice.SetRenderTarget(BufferTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Draw.Invoke(Main.spriteBatch);

            Main.spriteBatch.End();

            CopyContents(t, BufferTarget);
        }

        public void Project(RenderTarget2D u, RenderTarget2D v, RenderTarget2D p, RenderTarget2D div)
        {
            RenderTargetBinding[] oldBindings = Main.graphics.GraphicsDevice.GetRenderTargets();

            DrawToTargetWithBuffer(div, (sb) =>
            {
                LinuxMod.NavierStokesEffect.Parameters["velocityXField"].SetValue(u);
                LinuxMod.NavierStokesEffect.Parameters["velocityYField"].SetValue(v);

                LinuxMod.NavierStokesEffect.Techniques[0].Passes[2].Apply();
                Main.spriteBatch.Draw(div, div.Bounds, Color.White);
            });

            DrawToTargetWithBuffer(p, (sb) =>
            {
                Main.spriteBatch.Draw(Main.magicPixel, p.Bounds, new Color(0, 0, 0.5f));
            });

            for (int i = 0; i < 3; i++)
            {
                DrawToTargetWithBuffer(p, (sb) =>
                {
                    LinuxMod.NavierStokesEffect.Parameters["divMap"].SetValue(div);

                    LinuxMod.NavierStokesEffect.Techniques[0].Passes[3].Apply();
                    Main.spriteBatch.Draw(p, p.Bounds, Color.White);
                });
            }

            DrawToTargetWithBuffer(u, (sb) =>
            {
                LinuxMod.NavierStokesEffect.Parameters["pMap"].SetValue(p);

                LinuxMod.NavierStokesEffect.Techniques[0].Passes[4].Apply();
                Main.spriteBatch.Draw(u, u.Bounds, Color.White);
            });

            DrawToTargetWithBuffer(v, (sb) =>
            {
                LinuxMod.NavierStokesEffect.Parameters["pMap"].SetValue(p);

                LinuxMod.NavierStokesEffect.Techniques[0].Passes[5].Apply();
                Main.spriteBatch.Draw(v, v.Bounds, Color.White);
            });

            /*
            */
            Main.graphics.GraphicsDevice.SetRenderTargets(oldBindings);
        }

        public void ResolveDensity()
        {
            //Diffuse
            RenderTargetBinding[] oldBindings = Main.graphics.GraphicsDevice.GetRenderTargets();

            Matrix m = Matrix.CreateOrthographicOffCenter(0, N, N, 0, 0, -1);
            m.M41 += -0.5f * m.M11;
            m.M42 += -0.5f * m.M22;

            LinuxMod.NavierStokesEffect.Parameters["MATRIX"].SetValue(m);

            LinuxMod.NavierStokesEffect.Parameters["dT"].SetValue(1/50f);
            LinuxMod.NavierStokesEffect.Parameters["N"].SetValue(50);

            Iterate(0, ref PreviousDensityTarget, DensityTarget, DiffusionRate);
            //Iterate(0, ref PreviousDensityTarget, DensityTarget, DiffusionRate);
            //Apply vector field
            AdVec(0, ref DensityTarget, PreviousDensityTarget, VectorTargetX, VectorTargetY);

            Main.graphics.GraphicsDevice.SetRenderTargets(oldBindings);
        }

        public void ResolveVelocity()
        {
            //Diffuse in both directions
            Iterate(1, ref PVectorTargetX, VectorTargetX, Viscocity);
            Iterate(2, ref PVectorTargetY, VectorTargetY, Viscocity);

            //see top of project
            Project(PVectorTargetX, PVectorTargetY, VectorTargetX, VectorTargetY);

            //Vector field moves along itself
            AdVec(1, ref VectorTargetX, PVectorTargetX, PVectorTargetX, PVectorTargetY);
            AdVec(2, ref VectorTargetY, PVectorTargetY, PVectorTargetX, PVectorTargetY);

            Project(VectorTargetX, VectorTargetY, PVectorTargetX, PVectorTargetY);
        }
        public void Iterate(int type, ref RenderTarget2D active, RenderTarget2D buffer, float diff)
        {
            RenderTargetBinding[] oldBindings = Main.graphics.GraphicsDevice.GetRenderTargets();
            Main.graphics.GraphicsDevice.SetRenderTarget(BufferTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            LinuxMod.NavierStokesEffect.Parameters["bufferTarget"].SetValue(buffer);
            LinuxMod.NavierStokesEffect.Parameters["visc"].SetValue(diff);
            LinuxMod.NavierStokesEffect.Parameters["resolution"].SetValue(new Vector2(1 / (float)active.Bounds.X, 1 / (float)active.Bounds.Y));

            LinuxMod.NavierStokesEffect.Techniques[0].Passes[0].Apply();
            Main.spriteBatch.Draw(active, active.Bounds, Color.White);

            Main.spriteBatch.End();

            CopyContents(active, BufferTarget);

            Main.graphics.GraphicsDevice.SetRenderTargets(oldBindings);
        }

        public void AdSource(in RenderTarget2D Source, in RenderTarget2D Add, Action<SpriteBatch> AddAction)
        {
            RenderTargetBinding[] oldBindings = Main.graphics.GraphicsDevice.GetRenderTargets();

            Main.graphics.GraphicsDevice.SetRenderTarget(Add);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            AddAction?.Invoke(Main.spriteBatch);

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(BufferTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            LinuxMod.NavierStokesEffect.Parameters["adDensity"].SetValue(Add);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            LinuxMod.NavierStokesEffect.Techniques[0].Passes[6].Apply();
            Main.spriteBatch.Draw(Source, Source.Bounds, Color.White);

            Main.spriteBatch.End();

            CopyContents(Source, BufferTarget);

            Main.graphics.GraphicsDevice.SetRenderTargets(oldBindings);
        }

        public void AdVec(int type, ref RenderTarget2D active, RenderTarget2D buffer, RenderTarget2D u, RenderTarget2D v)
        {
            RenderTargetBinding[] oldBindings = Main.graphics.GraphicsDevice.GetRenderTargets();

            Main.graphics.GraphicsDevice.SetRenderTarget(BufferTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            LinuxMod.NavierStokesEffect.Parameters["bufferTarget"].SetValue(buffer);
            LinuxMod.NavierStokesEffect.Parameters["velocityXField"].SetValue(u);
            LinuxMod.NavierStokesEffect.Parameters["velocityYField"].SetValue(v);
            LinuxMod.NavierStokesEffect.Parameters["resolution"].SetValue(new Vector2(1 / (float)active.Bounds.X, 1 / (float)active.Bounds.Y));

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            LinuxMod.NavierStokesEffect.Techniques[0].Passes[1].Apply();
            Main.spriteBatch.Draw(active, active.Bounds, Color.White);

            Main.spriteBatch.End();

            CopyContents(active, BufferTarget);

            Main.graphics.GraphicsDevice.SetRenderTargets(oldBindings);
        }

        public void Draw(SpriteBatch sb)
        {
            Main.spriteBatch.Draw(VectorTargetX, GlobalSpace.ForDraw(), Color.White);
            LUtils.DrawRectangle(new Rectangle((int)(GlobalSpace.X - Main.screenPosition.X), (int)(GlobalSpace.Y - Main.screenPosition.Y), CellSize * N, CellSize * N), Color.Red, 1);

            Main.spriteBatch.Draw(VectorTargetY, GlobalSpace.ForDraw() + new Vector2(CellSize * N, 0), Color.White);
            LUtils.DrawRectangle(new Rectangle((int)(GlobalSpace.X - Main.screenPosition.X + CellSize * N), (int)(GlobalSpace.Y - Main.screenPosition.Y), CellSize * N, CellSize * N), Color.Red, 1);

            Main.spriteBatch.Draw(PreviousDensityTarget, GlobalSpace.ForDraw() + new Vector2(CellSize * N / 2, CellSize * N), Color.White);
            LUtils.DrawRectangle(new Rectangle((int)(GlobalSpace.X - Main.screenPosition.X + CellSize * N / 2), (int)(GlobalSpace.Y - Main.screenPosition.Y + CellSize * N), CellSize * N, CellSize * N), Color.Red, 1);
        }

        public void Dispose()
        {
            DensityTable = null;

            DensityTarget.Dispose();
            PreviousDensityTarget.Dispose();

            VectorTargetX.Dispose();
            PVectorTargetX.Dispose();

            VectorTargetY.Dispose();
            PVectorTargetY.Dispose();
        }
        #region CPU
        public void dEaseOut(float easeout)
        {
            for (int i = 0; i < DensityTable.Length; i++)
            {
                DensityTable[i] = MathHelper.Clamp(DensityTable[i] - easeout, 0, 255);
            }
        }

        public void ConfigureOcclusion(int type, ref float[] array)
        {
            // a bunch of boundary cases, there is unfortunately no clever way of doing this for all permutations afaik
            //stevie will probably figure it out

            /*
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
            */

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
                    div[XY(i, j)] = -0.5f * h * (u[XY(i + 1, j)] - u[XY(i - 1, j)] + v[XY(i, j + 1)] - v[XY(i, j - 1)]);
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
                ConfigureOcclusion(0, ref p);
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
                    float Y = jfloat - (v[XY(i, j)] * visc);

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
        #endregion
    }
}


