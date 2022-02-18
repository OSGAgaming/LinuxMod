
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace LinuxMod.Core.Assets
{
    public static class Asset
    {
        public static Texture2D GetTexture(string Path) => GetInstance<LinuxMod>().GetTexture("Assets/" + Path);

        public static Texture2D GetMiscTexture(string Path) => 
            Texture2D.FromStream(Main.graphics.GraphicsDevice, File.Open($@"{Main.SavePath}\Mod Sources\LinuxMod\Assets\" + Path, FileMode.Open));

        public const string ModelDirectory = "Models/";

        public const string NoiseDirectory = "Noise/";
    }
}
