using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using IUpdateable = LinuxMod.Core.Mechanics.Interfaces.IUpdateable;

namespace LinuxMod.Core.Mechanics
{
    public interface IPhysicsModule : IUpdateable
    {
        PhysicsObject Object { get; set; }
    }
}