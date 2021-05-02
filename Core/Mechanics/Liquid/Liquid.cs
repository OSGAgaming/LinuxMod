using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class Liquid : IComponent
    {
        public int accuracy;

        public Vector2[] Pos;
        public Vector2[] PosDampened;
        private Vector2[] accel;
        private Vector2[] vel;
        private Vector2[] targetHeight;

        public Rectangle frame;

        public float[] disLeft;
        private float[] disRight;
        public float dampening;
        public float constant;
        public float viscosity;

        [NonSerialized]
        public Color color = Color.LightBlue;
        public void SetDampeningTo(float dampening) => this.dampening = dampening;
        public void SetFrame(Rectangle vertices) => frame = vertices;
        public void Update()
        {
            Player entity = Main.LocalPlayer;

            Point pos = entity.position.ToPoint();
            Point dims = new Point(entity.width, entity.height);

            Rectangle playerFrame = new Rectangle(pos.X, pos.Y, dims.X, dims.Y);

            float preContact = playerFrame.Bottom - entity.velocity.Y * entity.velocity.Y;

            if (preContact < frame.Y && frame.Intersects(playerFrame))
                SplashPerc((entity.Center.X - frame.X) / frame.Width, new Vector2(entity.velocity.X / 4, entity.velocity.Y/4f));

            if (frame.Intersects(playerFrame))
            {
                Vector2 v = new Vector2(Math.Abs(entity.velocity.X), Math.Abs(entity.velocity.Y));
                SplashPerc((entity.Center.X - frame.X + entity.velocity.X) / frame.Width, new Vector2(0, -v.X / 4 * Main.rand.NextFloat(1, 1.5f)));
                SplashPerc((entity.Center.X - frame.X - entity.velocity.X) / frame.Width, new Vector2(0, v.X / 7 * Main.rand.NextFloat(1, 1.5f)));
            }

            for (int i = 0; i < accuracy + 1; i++)
            {
                Pos[i].X += vel[i].X;
                Pos[i].Y += vel[i].Y;
                vel[i].X += accel[i].X;
                vel[i].Y += accel[i].Y;
                accel[i].X = (targetHeight[i].X - Pos[i].X) / constant - (vel[i].X * dampening);
                accel[i].Y = (targetHeight[i].Y - Pos[i].Y) / constant - (vel[i].Y * dampening);
            }
            for (int i = 0; i < accuracy + 1; i++)
            {
                if (i > 0)
                {
                    disLeft[i] = (Pos[i].Y - Pos[i - 1].Y) * viscosity;
                    vel[i - 1].Y += disLeft[i];
                    Pos[i - 1].Y += disLeft[i];
                    disLeft[i] = (Pos[i].X - Pos[i - 1].X) * viscosity;
                    vel[i - 1].X += disLeft[i];
                    Pos[i - 1].X += disLeft[i];
                }
                if (i < accuracy)
                {
                    disRight[i] = (Pos[i].Y - Pos[i + 1].Y) * viscosity;
                    vel[i + 1].Y += disRight[i];
                    Pos[i + 1].Y += disRight[i];
                    disLeft[i] = (Pos[i].X - Pos[i + 1].X) * viscosity;
                    vel[i + 1].X += disLeft[i];
                    Pos[i + 1].X += disLeft[i];
                }
                float dY = Pos[i].Y - frame.Top;
                PosDampened[i].X = Pos[i].X;
                PosDampened[i].Y = frame.Top + dY * 0.5f;
            }

            Primitives.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Primitives.Draw(spriteBatch);
        }
        public void Splash(int index, float speed) => vel[index].Y = speed;
        public void SplashPerc(float perc, Vector2 speed) => vel[(int)(MathHelper.Clamp(perc, 0, 1) * accuracy)] += speed;

        private WaterPrimtives Primitives;
        public void Initialize()
        {
            disLeft = new float[accuracy + 1];
            disRight = new float[accuracy + 1];
            Pos = new Vector2[accuracy + 1];
            PosDampened = new Vector2[accuracy + 1];
            vel = new Vector2[accuracy + 1];
            accel = new Vector2[accuracy + 1];
            targetHeight = new Vector2[accuracy + 1];

            for (int i = 0; i < accuracy + 1; i++)
            {
                targetHeight[i].Y = frame.Y;
            }

            for (int i = 0; i < accuracy + 1; i++)
            {
                targetHeight[i].X = i * (frame.Width / (float)accuracy) + frame.X;
            }

            for (int i = 0; i < accuracy + 1; i++)
            {
                Pos[i].Y = frame.Y;
            }

            for (int i = 0; i < accuracy + 1; i++)
            {
                Pos[i].X = i * (frame.Width / (float)accuracy) + frame.X;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                Primitives = new WaterPrimtives(this);
            }
        }
    }
}


