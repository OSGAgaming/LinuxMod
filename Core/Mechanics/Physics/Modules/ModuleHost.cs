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
    public class ModuleHost<T> : IModuleHost where T : IPhysicsModule
    {
        public IList<T> Objects = new List<T>();

        public void AddObject(T obj) => Objects.Add(obj);
    }
}