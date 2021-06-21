using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Verlet;
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
    public class ControlModule : Module
    {
        private float Velocity { get; set; }

        public override void Update()
        {
            if(Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Object.Center.X += Velocity;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Object.Center.X -= Velocity;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Object.Center.Y -= Velocity;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Object.Center.Y += Velocity;
            }
        }

        public override void Load()
        {
           
        }

        public override void Draw(SpriteBatch sb)
        {
            
        }
        public ControlModule(float velocity)
        {
            GetHost<ControlModule>().AddObject(this);
            Velocity = velocity;
        }
    }
}