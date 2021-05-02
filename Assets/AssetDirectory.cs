
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static Terraria.ModLoader.ModContent;

namespace LinuxMod.Core.Assets
{
    public static class Asset
    {
       public static Texture2D GetTexture(string Path) => GetInstance<LinuxMod>().GetTexture("Assets/" + Path);
    }
}
