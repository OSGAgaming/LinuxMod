using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class PhysicsObjectHost : IComponent
    {
        public List<PhysicsObject> Objects = new List<PhysicsObject>();

        public void Draw(SpriteBatch spritebatch)
        {
            foreach(PhysicsObject module in Objects)
            {
                module.Draw(spritebatch);
            }
        }

        public void Update()
        {
            foreach(PhysicsObject module in Objects)
            {
                module.Update();
            }
        }

        public void AddObject(PhysicsObject Object) => Objects.Add(Object);
    }
}