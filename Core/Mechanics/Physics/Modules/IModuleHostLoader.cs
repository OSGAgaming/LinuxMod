
using System;

namespace LinuxMod.Core.Mechanics
{
    public static class ModuleHostLoader
    {
        public static void Load()
        {
            Type[] Mechanics = LUtils.GetInheritedClasses(typeof(Module));
            Type classType = typeof(ModuleHost<>);

            foreach (Type type in Mechanics)
            {
                Type Generic = classType.MakeGenericType(type);
                IModuleHost m = (IModuleHost)Activator.CreateInstance(Generic);

                Module.HostCache.Add(m);
            }
        }
    }

}