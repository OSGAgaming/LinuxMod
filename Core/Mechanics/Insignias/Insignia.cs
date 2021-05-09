using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class PNode
    {
        public Vector2 Position;
        public float Progression;

        public PNode(Vector2 Position, float Progression)
        {
            this.Position = Position;
            this.Progression = Progression;
        }
    }
    public class Insignia
    {
        public PNode[] KeyPoints;

        protected virtual void Ability() { }

    }
}