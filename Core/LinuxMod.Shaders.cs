
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
        public static Effect LightingBufferEffect;
        public static Effect PrimitiveShaders;
        public static Effect WaterWallReflection;

        static void QuickLoadShader(string Path)
        {
            Ref<Effect> Reference = new Ref<Effect>(ModContent.GetInstance<LinuxMod>().GetEffect(Path));

            string End = Path.Split('/')[1];
            string DictEntry = "Linux:" + End;

            Filters.Scene[DictEntry] = new Filter(new ScreenShaderData(Reference, "P1"), EffectPriority.VeryHigh);
            Filters.Scene[DictEntry].Load();
        }
        internal static void ShaderLoading()
        {
            PrimitiveShaders = ModContent.GetInstance<LinuxMod>().GetEffect("Effects/PrimitiveShader");

            QuickLoadShader("Effects/Viginette");
            QuickLoadShader("Effects/WaterWallReflection");
            QuickLoadShader("Effects/Sewers");

        }
    }
}