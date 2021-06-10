using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class PhysicsRigidBody : Module
    {
        public float Gravity { get; set; }

        public override void Update()
        {
            Object.Velocity.Y += Gravity;
        }

        public PhysicsRigidBody(float gravity)
        {
            Gravity = gravity;
        }
    }
}