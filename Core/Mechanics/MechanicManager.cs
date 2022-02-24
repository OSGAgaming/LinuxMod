using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        public static List<Mechanic> MechanicsCache;
        public static void Load()
        {
            MechanicsCache = new List<Mechanic>();

            Type[] Mechanics = LinuxTechTips.GetInheritedClasses(typeof(Mechanic));
            foreach (Type type in Mechanics)
            {
                Mechanic m = (Mechanic)Activator.CreateInstance(type);
                m.Load();
                MechanicsCache.Add(m);
            }
        }

        public static void Unload()
        {
            foreach (Mechanic type in MechanicsCache)
                type.Unload();

            MechanicsCache.Clear();
            MechanicsCache = null;
        }
    }
}