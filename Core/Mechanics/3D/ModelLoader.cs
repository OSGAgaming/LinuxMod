using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using SkinnedModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using System.Diagnostics;

namespace LinuxMod.Core.Mechanics
{
    public class ModelPathAttribute : Attribute
    {
        public string target;
        public ModelPathAttribute(string type)
        {
            target = type;
        }
    }

    public static class ModelLoader
    {
        public static Model Cube;
        public static Model Planet;
        [ModelPath("Planet")]
        public static Model Clouds;
        public static Model Skybox;
        public static Model SeamapSkybox;

        public static ContentManager contentManager;
        public static MethodInfo create_ContentReader;
        public static MethodInfo readAsset;

        public static void Load()
        {
            InitializeContentReader();

            FieldInfo[] Models = typeof(ModelLoader).GetFields();
            for(int i = 0; i < Models.Length; i++)
            {
                FieldInfo fi = Models[i];

                if (fi.FieldType == typeof(Model))
                {
                    ModelPathAttribute mpa;
                    if (fi.TryGetCustomAttribute(out mpa))
                    {
                        fi.SetValue(null, LoadModel(out _, mpa.target));
                        continue;
                    }
                    fi.SetValue(null, LoadModel(out _, fi.Name));
                }
            }
        }

        public static void Unload()
        {
            contentManager = null;
            create_ContentReader = null;
            readAsset = null;

            FieldInfo[] Models = typeof(ModelLoader).GetFields();
            foreach (FieldInfo fi in Models)
            {
                if (fi.FieldType == typeof(Model))
                {
                    fi.SetValue(null, null);
                }
            }
        }

        public static void InitializeContentReader()
        {
            contentManager = new ContentManager(Main.ShaderContentManager.ServiceProvider);
            create_ContentReader = typeof(ContentReader).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Static);

            readAsset = typeof(ContentReader).GetMethod("ReadAsset", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(typeof(object));
        }

        public static Model LoadModel(out Model model, string Path)
        {
            byte[] file = ModContent.GetFileBytes($"LinuxMod/Assets/Models/{Path}.urmom");
            model = LoadAsset<Model>(new MemoryStream(file));

            return model;
        }

        public static T LoadAsset<T>(Stream stream)
        {
            using (ContentReader contentReader = (ContentReader)create_ContentReader.Invoke(null, new object[] { contentManager, stream, "UntexturedSphere", null }))
            {
                var thi = readAsset.Invoke(contentReader, null);
                return (T)thi;
            }
        }
    }
}