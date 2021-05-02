
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
        internal static void ShaderLoading()
        {
            PrimitiveShaders = ModContent.GetInstance<LinuxMod>().GetEffect("Effects/PrimitiveShader");
            Ref<Effect> screenRef = new Ref<Effect>(ModContent.GetInstance<LinuxMod>().GetEffect("Effects/Viginette"));
            Ref<Effect> screenRef2 = new Ref<Effect>(ModContent.GetInstance<LinuxMod>().GetEffect("Effects/WaterWallReflection"));

            Filters.Scene["Linux:Viginette"] = new Filter(new ScreenShaderData(screenRef, "Viginette"), EffectPriority.VeryHigh);
            Filters.Scene["Linux:Viginette"].Load();

            Filters.Scene["Linux:WaterWallReflection"] = new Filter(new ScreenShaderData(screenRef2, "Func1"), EffectPriority.VeryHigh);
            Filters.Scene["Linux:WaterWallReflection"].Load();
        }
    }
}