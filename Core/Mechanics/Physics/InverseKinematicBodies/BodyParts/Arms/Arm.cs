using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public abstract class Arm : BodyPart
    {
        public abstract string OtherArm { get; }
        public virtual int StaticSide { get; }
        public int Side = 1;
        public int ArmSpan = 30;
        public bool Moving;

        public bool IsSupporting;
        public bool SupportListener;
        public Vector2 SupportPoint;

        float JointHori;
        int Delay;
        public Vector2 Joint;
        public Vector2 Ledge;
        public Vector2 Target => Parent.Center + new Vector2(ArmSpan * Side * StaticSide - JointHori * 3, 30 - Math.Abs(JointHori) * 4);

        Vector2[] CorrectArm(Vector2 feetVec, Vector2 jointVec)
        {
            float dx = feetVec.X - jointVec.X;
            float dy = feetVec.Y - jointVec.Y;
            float currentLength = (float)Math.Sqrt(dx * dx + dy * dy);
            float deltaLength = currentLength - (ArmSpan * 0.5f);
            float perc = (deltaLength / (float)currentLength) * 0.5f;
            float offsetX = perc * dx;
            float offsetY = perc * dy;
            Vector2 F = new Vector2(feetVec.X + offsetX, feetVec.Y + offsetY);
            Vector2 J = new Vector2(jointVec.X + offsetX, jointVec.Y + offsetY);

            return new Vector2[] { F, J };
        }

        public void Switch()
        {
            if(Delay == 0)
            Side *= -1;

            Delay = 3;
        }

        public void FindLedge()
        {
            for (int i = 0; i < 1; i++)
            {
                float Y = LUtils.TileCheckVertical(new Vector2(Center.X + 16 * i, Center.Y - 140), 1, 20) * 16;
                float X = Center.X + 16 * i;

                if(Y < Parent.Center.Y - 40 && Y != 0)
                {
                    if (!Framing.GetTileSafely(new Vector2(X, Y - 16).ToTileCoordinates()).active()
                        && !Framing.GetTileSafely(new Vector2(X - 16, Y).ToTileCoordinates()).active())
                    {
                        Ledge = new Vector2(X, Y);
                        Parent.Cycle = AnimationCycle.Hoist;
                    }
                    break;
                }
                else
                {
                    Ledge = new Vector2(-1);
                    Parent.Cycle = AnimationCycle.Walking;
                }
            }
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            LUtils.DrawCircle(Center.ForDraw(), new Vector2(5), Color.White);
            LUtils.DrawCircle(Joint.ForDraw(), new Vector2(5), Color.White);
            LUtils.DrawCircle(Ledge.ForDraw(), new Vector2(10), Color.Orange);

            LUtils.DrawLine(Joint.ForDraw(), Parent.Center.ForDraw() + new Vector2(0, -60).RotatedBy(Parent.UpperBodyRotation), Color.Purple);
            LUtils.DrawLine(Joint.ForDraw(), Center.ForDraw(), Color.Purple);
        }
        public override void Update()
        {
            if(!Parent.UpperBodyObstructionState && SupportListener && !(Parent.Get(OtherArm) as Arm).IsSupporting)
            {
                int Y = LUtils.TileCheckVertical(Parent.Center + new Vector2(Parent.DeltaCenter.X * 2,0), -1, 50);
                IsSupporting = true;
                SupportPoint = new Vector2(Parent.Center.X + Parent.DeltaCenter.X * 2, Y * 16);
            }

            if (Parent.UpperBodyObstructionState && !SupportListener)
            {
                IsSupporting = false;
            }

            if (Parent.Cycle == AnimationCycle.Walking)
            {
                if (Delay > 0) Delay--;

                if(!IsSupporting) Center += (Target - Center) / 11f;
                else Center += (SupportPoint - Center) / 11f;
                FindLedge();

                Joint = CorrectArm(Parent.Center + new Vector2(0, -60), Joint + new Vector2(JointHori, Math.Abs(JointHori)))[1];
                Joint = CorrectArm(Center, Joint + new Vector2(JointHori, Math.Abs(JointHori)))[1];

            }
            else if(Parent.Cycle == AnimationCycle.Hoist)
            {
                if (Ledge != new Vector2(-1))
                {
                    Center += (Ledge - Center) / 7f;
                    Parent.Get(OtherArm).Center += (Ledge - Parent.Get(OtherArm).Center) / 20f;
                    (Parent.Get(OtherArm) as Arm).Ledge = Ledge;
                }

                Joint = CorrectArm(Parent.Center + new Vector2(0, -60).RotatedBy(Parent.UpperBodyRotation), Joint + new Vector2(JointHori, Math.Abs(JointHori) + Parent.DeltaCenter.Y))[1];
                Joint = CorrectArm(Center, Joint + new Vector2(JointHori, Math.Abs(JointHori) + Parent.DeltaCenter.Y))[1];
            }

            JointHori += (Parent.DeltaCenter.X * -7 - JointHori) / 16f;

            if (Vector2.DistanceSquared(Target, Center) < 60 * 60)
            {
                Moving = false;
            }
            else
            {
                Moving = true;
            }

            SupportListener = Parent.UpperBodyObstructionState; 
        }
    }
}