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
        public int Lifetime => 1000;
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

            for (int i = 0; i < DensityTarget.Width; i++)
            {
                for (int j = 0; j < DensityTarget.Height; j++)
                {
                    c[i + j * DensityTarget.Width] = new Color(0, 0f, 0.5f);
                    cV[i + j * DensityTarget.Width] = new Color(0, 0, 0.5f);
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

            DensityTarget.SetData(c);
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

            Matrix m = Matrix.CreateOrthographicOffCenter(0, N, N, 0, 0, -1);
            m.M41 += -0.5f * m.M11;
            m.M42 += -0.5f * m.M22;

            LinuxMod.NavierStokes.Parameters["MATRIX"].SetValue(m);
            LinuxMod.NavierStokes.Parameters["dT"].SetValue(1 / (float)(N / 2));
            LinuxMod.NavierStokes.Parameters["N"].SetValue(N / 2);

            LinuxMod.NavierStokes.Parameters["relativeScreenPos"].SetValue((GlobalSpace - Main.screenPosition) / new Vector2(Main.screenWidth, Main.screenHeight));
            LinuxMod.NavierStokes.Parameters["MistDims"].SetValue(N);
            LinuxMod.NavierStokes.Parameters["boundaries"].SetValue(Targets.Instance.ScaledTileTarget);
            LinuxMod.NavierStokes.Parameters["ScreenDims"].SetValue(Targets.Instance.ScaledTileTarget.Bounds.Size());
        }

        public Color ConvertToColor(float input)
        {
            float sign = (Math.Sign(input) + 1) / 2f;

            return new Color((1 - sign) * Math.Abs(input), sign * Math.Abs(input), 0);
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

                float angle = TimeAlive * 0.01f;
                Vector2 v = angle.ToRotationVector2();
                v *= 1f;

                float signX = (Math.Sign(v.X) + 1) / 2f;
                float signY = (Math.Sign(v.Y) + 1) / 2f;

                float absX = Math.Abs(v.X);
                float absY = Math.Abs(v.Y);

                //Main.NewText("X: " + new Vector2((1 - signX) * absX, signX * absX));
                //Main.NewText("Y: " + new Vector2((1 - signY) * absY, signY * absY));

                AdSource(DensityTarget, AddVecSourceBuffer, (sb) =>
                {
                    sb.Draw(Main.magicPixel, new Rectangle(N / 2 + 20, N / 2 - 20, 4, 4), new Color(0f, 1f, 0));
                });

                AdSource(VectorTargetX, AddVecSourceBuffer, (sb) =>
                {
                    sb.Draw(Main.magicPixel, new Rectangle(N / 2 + 20, N / 2 - 20, 8, 8), ConvertToColor(v.X));
                });

                AdSource(VectorTargetY, AddVecSourceBuffer, (sb) =>
                {
                    sb.Draw(Main.magicPixel, new Rectangle(N / 2 + 20, N / 2 - 20, 8, 8), ConvertToColor(v.Y));
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
                LinuxMod.NavierStokes.Parameters["velocityXField"].SetValue(u);
                LinuxMod.NavierStokes.Parameters["velocityYField"].SetValue(v);

                LinuxMod.NavierStokes.Techniques[0].Passes[2].Apply();
                sb.Draw(div, div.Bounds, Color.White);
            });

            DrawToTargetWithBuffer(p, (sb) =>
            {
                Rectangle rect = new Rectangle(2, 2, p.Bounds.X - 4, p.Bounds.Y - 4);
                sb.Draw(Main.magicPixel, rect, new Color(0, 0, 0.5f));
            });

            ConfigureOcclusion(0, div);
            ConfigureOcclusion(0, p);

            for (int i = 0; i < 3; i++)
            {
                DrawToTargetWithBuffer(p, (sb) =>
                {
                    LinuxMod.NavierStokes.Parameters["divMap"].SetValue(div);

                    LinuxMod.NavierStokes.Techniques[0].Passes[3].Apply();
                    sb.Draw(p, p.Bounds, Color.White);
                });

                ConfigureOcclusion(0, p);
            }

            DrawToTargetWithBuffer(u, (sb) =>
            {
                LinuxMod.NavierStokes.Parameters["pMap"].SetValue(p);

                LinuxMod.NavierStokes.Techniques[0].Passes[4].Apply();
                sb.Draw(u, u.Bounds, Color.White);
            });

            DrawToTargetWithBuffer(v, (sb) =>
            {
                LinuxMod.NavierStokes.Parameters["pMap"].SetValue(p);

                LinuxMod.NavierStokes.Techniques[0].Passes[5].Apply();
                sb.Draw(v, v.Bounds, Color.White);
            });

            ConfigureOcclusion(1, u);
            ConfigureOcclusion(2, v);
            /*
            */
            Main.graphics.GraphicsDevice.SetRenderTargets(oldBindings);
        }

        public void ResolveDensity()
        {
            //Diffuse
            RenderTargetBinding[] oldBindings = Main.graphics.GraphicsDevice.GetRenderTargets();

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

            LinuxMod.NavierStokes.Parameters["bufferTarget"].SetValue(buffer);
            LinuxMod.NavierStokes.Parameters["visc"].SetValue(diff);
            LinuxMod.NavierStokes.Parameters["resolution"].SetValue(new Vector2(1 / (float)active.Bounds.X, 1 / (float)active.Bounds.Y));

            LinuxMod.NavierStokes.Techniques[0].Passes[0].Apply();
            Main.spriteBatch.Draw(active, active.Bounds, Color.White);

            Main.spriteBatch.End();

            CopyContents(active, BufferTarget);

            ConfigureOcclusion(type, active);

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

            LinuxMod.NavierStokes.Parameters["adDensity"].SetValue(Add);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            LinuxMod.NavierStokes.Techniques[0].Passes[6].Apply();
            Main.spriteBatch.Draw(Source, Source.Bounds, Color.White);

            Main.spriteBatch.End();

            CopyContents(Source, BufferTarget);

            Main.graphics.GraphicsDevice.SetRenderTargets(oldBindings);
        }

        public void AdVec(int type, ref RenderTarget2D active, RenderTarget2D buffer, RenderTarget2D u, RenderTarget2D v)
        {
            LinuxMod.NavierStokes.Parameters["bufferTarget"].SetValue(buffer);
            LinuxMod.NavierStokes.Parameters["velocityXField"].SetValue(u);
            LinuxMod.NavierStokes.Parameters["velocityYField"].SetValue(v);
            LinuxMod.NavierStokes.Parameters["resolution"].SetValue(new Vector2(1 / (float)active.Bounds.X, 1 / (float)active.Bounds.Y));

            RenderTargetBinding[] oldBindings = Main.graphics.GraphicsDevice.GetRenderTargets();

            Main.graphics.GraphicsDevice.SetRenderTarget(BufferTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            LinuxMod.NavierStokes.Techniques[0].Passes[1].Apply();
            Main.spriteBatch.Draw(active, active.Bounds, Color.White);

            Main.spriteBatch.End();

            CopyContents(active, BufferTarget);

            ConfigureOcclusion(type, active);

            Main.graphics.GraphicsDevice.SetRenderTargets(oldBindings);
        }

        public void ConfigureOcclusion(int type, RenderTarget2D buffer)
        {

            LinuxMod.NavierStokes.Parameters["occlusionType"].SetValue(type);

            void Pass(int pass)
            {
                Main.graphics.GraphicsDevice.SetRenderTarget(BufferTarget);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                LinuxMod.NavierStokes.Techniques[0].Passes[pass].Apply();
                Main.spriteBatch.Draw(buffer, buffer.Bounds, Color.White);

                Main.spriteBatch.End();

                CopyContents(buffer, BufferTarget);
            }

            RenderTargetBinding[] oldBindings = Main.graphics.GraphicsDevice.GetRenderTargets();

            Pass(8);
            Pass(9);
            Pass(10);
            Pass(11);
            Pass(12);
            Pass(13);

            Main.graphics.GraphicsDevice.SetRenderTargets(oldBindings);

        }

        public void Draw(SpriteBatch sb)
        {
            /*
            sb.Draw(VectorTargetX, GlobalSpace.ForDraw(), Color.White);
            LinuxTechTips.DrawRectangle(new Rectangle((int)(GlobalSpace.X - Main.screenPosition.X), (int)(GlobalSpace.Y - Main.screenPosition.Y), CellSize * N, CellSize * N), Color.Red, 1);

            sb.Draw(VectorTargetY, GlobalSpace.ForDraw() + new Vector2(CellSize * N, 0), Color.White);
            LinuxTechTips.DrawRectangle(new Rectangle((int)(GlobalSpace.X - Main.screenPosition.X + CellSize * N), (int)(GlobalSpace.Y - Main.screenPosition.Y), CellSize * N, CellSize * N), Color.Red, 1);

            sb.Draw(PreviousDensityTarget, GlobalSpace.ForDraw() + new Vector2(CellSize * N / 2, CellSize * N), Color.White);
            LinuxTechTips.DrawRectangle(new Rectangle((int)(GlobalSpace.X - Main.screenPosition.X + CellSize * N / 2), (int)(GlobalSpace.Y - Main.screenPosition.Y + CellSize * N), CellSize * N, CellSize * N), Color.Red, 1);
            */

            sb.Draw(Targets.Instance.ScaledTileTarget, Vector2.Zero, Color.White);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            LinuxMod.NavierStokes.Techniques[0].Passes[7].Apply();
            sb.Draw(DensityTarget, GlobalSpace.ForDraw(), Color.White);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);
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
                    /*
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
                    */
                }
            }
        }

        //Curently the tile boundaries on the GPU are buggy and I don't know why.

        //E X P E C T E D     A C T U A L

        //X X X X X X X X     X X X X X X X X 
        //X X 4 4 4 4 X X     X X 4 4 4 4 X X 
        //X 3 0 0 0 0 2 X     X 3 0 0 0 0 2 X
        //X 3 0 0 0 0 2 X     X 3 0 0 0 0 2 X
        //X 3 0 0 0 0 2 X     X 3 0 0 0 0 2 X
        //X X 1 1 1 1 X X     X X 1 1 1 1 X X
        //X X X X X X X X     X X X X X X X X

        //This is most likely due to the boundaries of the texture overlapping because of the modulus

        //TODO: Fix this is in 1.4

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


