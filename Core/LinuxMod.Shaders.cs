
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace LinuxMod.Core
{
    public class ShaderPathAttribute : Attribute
    {
        public string target;
        public bool load;
        public string pass;

        public ShaderPathAttribute(string type, bool loadToDict = false, string pass = "P1")
        {
            target = type;
            load = loadToDict;
            this.pass = pass;
        }
    }

    public partial class LinuxMod : Mod
    {
        public static Effect PrimitiveShader;
        [ShaderPath("ModelShaders/ExampleModelShader")]
        public static Effect ExampleModelShader;
        public static Effect ColourLimit;
        public static Effect NavierStokes;
        public static Effect WaterOcclusion;
        public static Effect WaterReflection;
        [ShaderPath("dUdVMap", true, "Reflect")]
        public static Effect dUdVMap;
        [ShaderPath("PixelationShader", true)]
        public static Effect PixelationShader;
        [ShaderPath("ModelShaders/NormalMapModelShader", true, "P1")]
        public static Effect NormalMapModelShader;

        public static void UnloadShaders()
        {
            FieldInfo[] Models = typeof(LinuxMod).GetFields();
            for (int i = 0; i < Models.Length; i++)
            {
                FieldInfo fi = Models[i];

                if (fi.FieldType == typeof(Effect))
                {
                    fi.SetValue(null, null);
                }
            }
        }

        public static void LoadShaders()
        {
            FieldInfo[] Models = typeof(LinuxMod).GetFields();
            for (int i = 0; i < Models.Length; i++)
            {
                FieldInfo fi = Models[i];

                if (fi.FieldType == typeof(Effect))
                {
                    ShaderPathAttribute spa;
                    if (fi.TryGetCustomAttribute(out spa))
                    {
                        fi.SetValue(null, ModContent.GetInstance<LinuxMod>().GetEffect($"Effects/{spa.target}"));
                        continue;
                    }
                    fi.SetValue(null, ModContent.GetInstance<LinuxMod>().GetEffect($"Effects/{fi.Name}"));
                }
            }
        }

        static void QuickLoadScreenShader(string Path)
        {
            string EffectPath = "Effects/ScreenShaders/" + Path;
            string DictEntry = "Linux:" + Path;

            Ref<Effect> Reference = new Ref<Effect>(ModContent.GetInstance<LinuxMod>().GetEffect(EffectPath));

            Filters.Scene[DictEntry] = new Filter(new ScreenShaderData(Reference, "P1"), EffectPriority.VeryHigh);
            Filters.Scene[DictEntry].Load();
        }

        static void LoadScreenShaders()
        {
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

                QuickLoadScreenShader(AlteredPath.Replace(".fx", ""));
            }

            FieldInfo[] Models = typeof(LinuxMod).GetFields();
            for (int i = 0; i < Models.Length; i++)
            {
                FieldInfo fi = Models[i];

                if (fi.FieldType == typeof(Effect))
                {
                    Effect effect;
                    ShaderPathAttribute spa;
                    if (fi.TryGetCustomAttribute(out spa))
                    {
                        effect = ModContent.GetInstance<LinuxMod>().GetEffect($"Effects/{spa.target}");
                        if (!spa.load) continue;

                        Ref<Effect> sRef = new Ref<Effect>(effect);

                        Filters.Scene[fi.Name] = new Filter(new ScreenShaderData(sRef, spa.pass), EffectPriority.VeryHigh);
                        Filters.Scene[fi.Name].Load();
                    }                    
                }
            }
        }
        internal static void ShaderLoading()
        {
            LoadShaders();
            LoadScreenShaders();
        }
    }
}