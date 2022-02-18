using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public enum AnimationCycle
    {
        Walking,
        Hoist
    }
    public class CorePart : PhysicsObject
    {
        public AnimationCycle Cycle = AnimationCycle.Walking;

        public float UpperBodyRotation;
        public bool UpperBodyObstructionState;
        public CorePart(Vector2 Spawn, float Velocity)
        {
            LoadModule(new ControlModule(Velocity));
            Center = Spawn;

            AppendBodyPart(new LeftLeg());
            AppendBodyPart(new RightLeg());

            AppendBodyPart(new RightArm());
            AppendBodyPart(new LeftArm());
        }

        public Dictionary<string, BodyPart> Parts = new Dictionary<string, BodyPart>();

        public void AppendBodyPart(BodyPart Part)
        {
            Part.Parent = this;
            Part.Center = Center;
            Parts.Add(Part.ID, Part);
        }

        public BodyPart Get(string PartID) => Parts[PartID];

        public void UpdateParts()
        {
            foreach(KeyValuePair<string, BodyPart> Parts in Parts)
            {
                Parts.Value.Update();
            }
            if(Parts.ContainsKey("L_Leg") && Parts.ContainsKey("R_Leg"))
            {
                float LowestY = Get("L_Leg").Center.Y;
                if(Get("R_Leg").Center.Y > LowestY) LowestY = Get("R_Leg").Center.Y;

                Center.Y += (LowestY - (Get("L_Leg") as Leg).LegLength - 16 - Center.Y) / 6f;

                if(!Keyboard.GetState().IsKeyDown(Keys.A) && !Keyboard.GetState().IsKeyDown(Keys.D))
                Center.X += (Get("L_Leg").Center.X/2 + Get("R_Leg").Center.X / 2 - Center.X) / 32f;

                if(Cycle == AnimationCycle.Hoist)
                {
                    if((Get("L_Arm") as Arm).Ledge.X != -1)
                    Center.X += ((Get("L_Arm") as Arm).Ledge.X - Center.X) /40f;
                }
            }
        }

        public void DrawParts(SpriteBatch spriteBatch)
        {
            foreach (KeyValuePair<string, BodyPart> Parts in Parts)
            {
                Parts.Value.Draw(spriteBatch);
            }

            LinuxTechTips.DrawCircle(Center.ForDraw(), new Vector2(10), Color.White);

            LinuxTechTips.DrawLine(Center.ForDraw(), (Center - new Vector2(0,100).RotatedBy(UpperBodyRotation)).ForDraw(), Color.Blue);

            UpperBodyObstructionState = Collision.CanHitLine(Center, 1, 1, Center - new Vector2(0, 100), 1, 1);

            if(!UpperBodyObstructionState)
            {
                UpperBodyRotation += (-0.4f - UpperBodyRotation) / 32f;
            }
            else
            {
                UpperBodyRotation += (0 - UpperBodyRotation) / 32f;
            }
        }
    }
}