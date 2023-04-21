using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System;

namespace LinuxMod.Core.Mechanics
{
    public class FakeNPC : Entity
    {
        public float rotation;

        public void Update() 
        {
            position += velocity;
            AI();
        }

        public virtual void AI() { }

        public virtual void OnSpawn() { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        public FakeNPC()
        {
            active = true;
            OnSpawn();
        }
    }
}