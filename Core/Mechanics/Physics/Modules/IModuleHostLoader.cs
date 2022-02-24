
using System;
using System.Collections.Generic;

namespace LinuxMod.Core.Mechanics
{
    public static class ModuleHostLoader
    {
        public static void Load()
        {
            Module.HostCache = new List<IModuleHost>();

            Type[] Mechanics = LinuxTechTips.GetInheritedClasses(typeof(Module));
            Type classType = typeof(ModuleHost<>);

            foreach (Type type in Mechanics)
            {
                Type Generic = classType.MakeGenericType(type);
                IModuleHost m = (IModuleHost)Activator.CreateInstance(Generic);

                Module.HostCache.Add(m);
            }
        }

        public static void Unload()
        {
            Module.HostCache.Clear();
            Module.HostCache = null;
        }
    }

}