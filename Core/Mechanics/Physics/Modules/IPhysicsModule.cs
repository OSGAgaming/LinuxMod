using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using IUpdate = LinuxMod.Core.Mechanics.Interfaces.IUpdate;

namespace LinuxMod.Core.Mechanics
{
    public interface IPhysicsModule : IUpdate
    {
        PhysicsObject Object { get; set; }
    }
}