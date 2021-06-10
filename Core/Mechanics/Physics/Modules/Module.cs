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
    public class Module : IPhysicsModule
    {
        public static List<IModuleHost> HostCache = new List<IModuleHost>();

        public static ModuleHost<T> GetHost<T>() where T : IPhysicsModule
        {
            foreach (IModuleHost Module in HostCache)
            {
                if(Module is ModuleHost<T>)
                {
                    return Module as ModuleHost<T>;
                }
            }

            return null;
        }

        public virtual void Update() { }

        public virtual void Load() { }

        public virtual void Draw(SpriteBatch sb) { }

        public PhysicsObject Object { get; set; }
    }
}