using LinuxMod.Core.Mechanics.Interfaces;
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
    public abstract class BodyPart : Entity, IComponent
    {
        public virtual string ID { get; } = "NaN";

        public CorePart Parent { get; set; }

        public virtual void Draw(SpriteBatch spritebatch) { }

        public virtual void Update() { }
    }
}