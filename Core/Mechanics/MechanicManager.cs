using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public static class AutoloadMechanics
    {

        public static void Load()
        {
            Type[] Mechanics = LUtils.GetInheritedClasses(typeof(Mechanic));
            foreach (Type type in Mechanics)
            {
                Mechanic m = (Mechanic)Activator.CreateInstance(type);
                m.Load();
            }
        }
    }
}