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

namespace LinuxMod.Core.Mechanics
{
    public class ModelLoader
    {
        public Model Cube;
        public Model Planet;
        public Model Clouds;

        public ContentManager contentManager;

        public MethodInfo create_ContentReader;

        public MethodInfo readAsset;

        public void Load()
        {
            InitializeContentReader();

            LoadModel(out Cube, "Cube");
            LoadModel(out Planet, "Planet");
            LoadModel(out Clouds, "Planet");
        }

        public void InitializeContentReader()
        {
            contentManager = new ContentManager(Main.ShaderContentManager.ServiceProvider);
            create_ContentReader = typeof(ContentReader).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Static);

            readAsset = typeof(ContentReader).GetMethod("ReadAsset", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(typeof(object));
        }

        public void LoadModel(out Model model, string Path)
        {
            byte[] file = ModContent.GetFileBytes($"LinuxMod/Assets/Models/{Path}.urmom");
            model = LoadAsset<Model>(new MemoryStream(file));
        }

        public T LoadAsset<T>(Stream stream)
        {
            using (ContentReader contentReader = (ContentReader)create_ContentReader.Invoke(null, new object[] { contentManager, stream, "UntexturedSphere", null }))
            {
                var thi = readAsset.Invoke(contentReader, null);
                return (T)thi;
            }
        }
        public void LoadModels()
        {
            Load();  
        }
    }
}