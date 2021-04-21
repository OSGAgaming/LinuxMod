
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
        internal static void ShaderLoading()
        {
            //LightingBufferEffect = ModContent.GetInstance<EEMod>().GetEffect("Effects/LightingBuffer");

            Ref<Effect> screenRef = new Ref<Effect>(ModContent.GetInstance<LinuxMod>().GetEffect("Effects/Viginette"));

            Filters.Scene["Linux:Viginette"] = new Filter(new ScreenShaderData(screenRef, "Viginette"), EffectPriority.VeryHigh);
            Filters.Scene["Linux:Viginette"].Load();
        }
    }
}