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
    public abstract class Leg : BodyPart
    {
        public abstract string OtherLeg { get; }

        public int LegLength = 100;
        public float StepTolerance = 30;
        public float StepLength = 60;
        public float KneeRoom = 70;
        public float Gravity = 0.05f;
        public float WalkCycleSpeed = 0.03f;
        public float WalkCycleHeight = 60;
        public float SafeLandDistance = 10;

        public bool isTakingStep = false;
        public float LengthTo;
        public Vector2 Outset;

        public float WalkCycle;
        public Vector2 Target;
        public Vector2 Joint;
        public float JointHori;

        public bool CanRise = true;
        public bool HasSteped = false;
        public float HoistSpeed;
        public Vector2 HoistPoint = Vector2.Zero;

        public override void Draw(SpriteBatch spritebatch)
        {
            int Sign = Math.Sign(Parent.DeltaCenter.X);
            float DiffX = Parent.Center.X - Center.X;

            LUtils.DrawCircle(Center.ForDraw(), new Vector2(20), Color.Lerp(Color.White, Color.Yellow, WalkCycle));
            LUtils.DrawCircle(Target.ForDraw(), new Vector2(14), Color.Yellow);
            LUtils.DrawCircle(Joint.ForDraw(), new Vector2(10), Color.Yellow);

            LUtils.DrawLine(Parent.Center.ForDraw(), Joint.ForDraw(), Color.Blue, 1);
            LUtils.DrawLine(Center.ForDraw(), Joint.ForDraw(), Color.Blue, 1);

            if (Math.Abs(DiffX) > StepTolerance / 2 && Math.Sign(DiffX) == Sign)
            {
                Vector2 HoriSightline = Parent.Center + new Vector2(StepLength * Sign + Parent.DeltaCenter.X * 10, KneeRoom);
                //LUtils.DrawLine(Parent.Center.ForDraw(), HoriSightline.ForDraw(), Color.Yellow, 1);

                int Vert = LUtils.TileCheckVertical(HoriSightline, 1, LegLength / 16 + 3);
               //LUtils.DrawLine(HoriSightline.ForDraw(), new Vector2(HoriSightline.X, Vert * 16).ForDraw(), Color.White, 1);
            }
        }

        Vector2[] CorrectLeg(Vector2 feetVec, Vector2 jointVec)
        {
            float dx = feetVec.X - jointVec.X;
            float dy = feetVec.Y - jointVec.Y;
            float currentLength = (float)Math.Sqrt(dx * dx + dy * dy);
            float deltaLength = currentLength - (LegLength * 0.5f);
            float perc = (deltaLength / (float)currentLength) * 0.5f;
            float offsetX = perc * dx;
            float offsetY = perc * dy;
            Vector2 F = new Vector2(feetVec.X + offsetX, feetVec.Y + offsetY);
            Vector2 J = new Vector2(jointVec.X + offsetX, jointVec.Y + offsetY);

            return new Vector2[] { F, J };
        }

        public void Step()
        {
            float DiffX = Parent.Center.X - Center.X;
            int Sign = Math.Sign(Parent.DeltaCenter.X);

            float ExtrapolatedXTarget = Parent.Center.X + StepLength * Sign + Parent.DeltaCenter.X * 20;
            int Vert = LUtils.TileCheckVertical(new Vector2(ExtrapolatedXTarget, Parent.Get(OtherLeg).Center.Y - KneeRoom), 1, LegLength*2 + 3);

            int Vert2 = LUtils.TileCheckVertical(Center, 1, LegLength/2);

            if (Math.Abs(DiffX) > StepTolerance && Math.Sign(DiffX) == Sign)
            {
                if (Vert != 0 && !isTakingStep)
                {
                    (Parent.Get("R_Arm") as Arm).Switch();
                    (Parent.Get("L_Arm") as Arm).Switch();

                    isTakingStep = true;
                    Target.Y = Vert * 16;
                    Target.X = ExtrapolatedXTarget;
                    Outset = Center;
                }
            }

            if (Vert2 - Center.Y > 5 && !isTakingStep)
            {
                velocity.Y += Gravity;
                Center += new Vector2(0, velocity.Y);
            }
            else
            {
                velocity.Y = 0;
            }
        }

        public void HoistBehaviour()
        {

        }

        public void StepBehaviour()
        {
            if (isTakingStep)
            {
                (Parent.Get(OtherLeg) as Leg).isTakingStep = false;
                Center = LUtils.TraverseBezier(Target, Outset, (Outset + Target) / 2 - new Vector2(0, WalkCycleHeight), MathHelper.SmoothStep(0,1,WalkCycle));

                if (WalkCycle < 1) WalkCycle += WalkCycleSpeed;

                if (Vector2.DistanceSquared(Target, Center) < SafeLandDistance * SafeLandDistance)
                {
                    isTakingStep = false;
                    WalkCycle = 0;
                    if (Parent.Cycle == AnimationCycle.Hoist) HasSteped = true;
                }
            }
            else
            {
                WalkCycle = 0;
            }
        }

        public override void Update()
        {
            float DiffX = Parent.Center.X - Center.X;

            if (Parent.Cycle == AnimationCycle.Walking)
            {
                StepBehaviour();
                Step();

                Joint = CorrectLeg(Parent.Center, Joint + new Vector2(JointHori, 0))[1];
                Joint = CorrectLeg(Center, Joint + new Vector2(JointHori, 0))[1];
            }
            else if(Parent.Cycle == AnimationCycle.Hoist)
            {
                float LedgeX = (Parent.Get("L_Arm") as Arm).Ledge.X;

                int Vert2 = LUtils.TileCheckVertical(new Vector2(LedgeX, Center.Y - 120), 1, 20);
               
                if (CanRise)
                {
                    HoistSpeed += 0.06f;

                    Center += (new Vector2(Parent.Center.X, Center.Y) - Center) / 16f;
                    

                    Joint = CorrectLeg(Parent.Center, Joint + new Vector2(JointHori, 0))[1];
                    Joint = CorrectLeg(Center, Joint + new Vector2(JointHori, 0))[1];

                    HoistPoint = new Vector2((Parent.Get("L_Arm") as Arm).Ledge.X, Vert2 * 16 - 16);
                    Point p = HoistPoint.ToTileCoordinates();

                    if (WorldGen.InWorld(p.X, p.Y, 10))
                    {
                        Tile tileUp = Framing.GetTileSafely(p);
                        if (!tileUp.active())
                        {
                            CanRise = false;
                            isTakingStep = true;
                            WalkCycle = 0;
                            Target = HoistPoint + new Vector2(0, 16);
                            if((Parent.Get(OtherLeg) as Leg).HasSteped) Target = HoistPoint + new Vector2(64, 16);
                        }
                    }
                }
                else
                {
                    HoistSpeed -= HoistSpeed / 8f;

                    if (!isTakingStep && !HasSteped)
                    {
                        CanRise = true;
                        Target = HoistPoint + new Vector2(40, 16);
                    }

                    Outset = Center;

                    StepBehaviour();

                    Joint = CorrectLeg(Parent.Center, Joint + new Vector2(JointHori, -HoistSpeed * 5 - 10))[1];
                    Joint = CorrectLeg(Center, Joint + new Vector2(JointHori, -HoistSpeed * 5 - 10))[1];

                    if (HasSteped && (Parent.Get(OtherLeg) as Leg).HasSteped)
                    {
                        Leg Otherleg = (Parent.Get(OtherLeg) as Leg);

                        HasSteped = false;
                        Otherleg.HasSteped = false;
                        CanRise = true;
                        Otherleg.CanRise = true;

                        Parent.Cycle = AnimationCycle.Walking;

                    }
                }

                Center += new Vector2(0, -(HoistSpeed * HoistSpeed * HoistSpeed));
            }

            

            JointHori += (Parent.DeltaCenter.X * 4 - JointHori) / 16f;
            //Center = Parent.Center + new Vector2(-10, 80);
        }
    }
}