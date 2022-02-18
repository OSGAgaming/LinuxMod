
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.IO;
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
        public static Effect ExampleModelShader;
        public static Effect ModelTargetEffect;
        public static Effect NavierStokesEffect;
        public static Effect WaterOcclusionEffect;
        public static Effect ReflectRefractEffect;
        public static Effect DUDVMap;

        static void QuickLoadScreenShader(string Path)
        {
            string EffectPath = "Effects/ScreenShaders/" + Path;
            string DictEntry = "Linux:" + Path;

            Ref<Effect> Reference = new Ref<Effect>(ModContent.GetInstance<LinuxMod>().GetEffect(EffectPath));

            Filters.Scene[DictEntry] = new Filter(new ScreenShaderData(Reference, "P1"), EffectPriority.VeryHigh);
            Filters.Scene[DictEntry].Load();
        }
        internal static void ShaderLoading()
        {
            PrimitiveShaders = ModContent.GetInstance<LinuxMod>().GetEffect("Effects/PrimitiveShader");
            ExampleModelShader = ModContent.GetInstance<LinuxMod>().GetEffect("Effects/ModelShaders/ExampleModelShader");
            ModelTargetEffect = ModContent.GetInstance<LinuxMod>().GetEffect("Effects/ColourLimit");
            NavierStokesEffect = ModContent.GetInstance<LinuxMod>().GetEffect("Effects/NavierStokes");
            WaterOcclusionEffect = ModContent.GetInstance<LinuxMod>().GetEffect("Effects/WaterOcclusion");
            ReflectRefractEffect = ModContent.GetInstance<LinuxMod>().GetEffect("Effects/WaterReflection");
            DUDVMap = ModContent.GetInstance<LinuxMod>().GetEffect("Effects/dUdVMap");

            string[] Shaders = Directory.GetFiles($@"{Main.SavePath}\Mod Sources\LinuxMod\Effects\ScreenShaders");
            for (int i = 0; i < Shaders.Length; i++)
            {
                string filePath = Shaders[i];

                if (filePath.Contains(".xnb") || 
                    filePath.Contains(".exe") ||
                    filePath.Contains(".dll")) continue;

                string charSeprator = @"ScreenShaders\";
                int Index = filePath.IndexOf(charSeprator) + charSeprator.Length;
                string AlteredPath = filePath.Substring(Index);

                QuickLoadScreenShader(AlteredPath.Replace(".fx",""));
            }
        }
    }
}