
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace LinuxMod.Core
{
    public partial class LinuxMod : Mod
    {
        public static ModHotKey InsigniaActivator;

        public void LoadHotkeys()
        {
            InsigniaActivator = RegisterHotKey("Activate Insignia Maker", "Z");
        }
    }
}