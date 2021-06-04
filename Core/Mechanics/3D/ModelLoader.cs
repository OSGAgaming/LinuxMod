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
        public static Model model;
        public static ContentManager contentManager;//if path is never accessed then use vanilla shader content manager instead

        public static MethodInfo create_ContentReader;
        //public ConstructorInfo[] contentReaderCtor;

        public static MethodInfo readAsset;
        public static MethodInfo readAssetExact;

        public static void Load()
        {
            contentManager = new ContentManager(Main.ShaderContentManager.ServiceProvider);
            create_ContentReader = typeof(ContentReader).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Static);

            readAsset = typeof(ContentReader).GetMethod("ReadAsset", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(typeof(object));
            readAssetExact = typeof(ContentReader).GetMethod("ReadObject", new Type[] { }).MakeGenericMethod(typeof(object));


        byte[] file = ModContent.GetFileBytes("LinuxMod/Assets/Models/Cube.urmom");
            //model type is 'Model' material type is 'BasicEffect'
            model = LoadAsset<Model>(new MemoryStream(file));
            //model.SetTexture(ModContent.GetTexture("Realms/Models/Sans Tex"));
        }

        public static T LoadAsset<T>(Stream stream)
        {
            using (ContentReader contentReader = (ContentReader)create_ContentReader.Invoke(null, new object[] { contentManager, stream, "UntexturedSphere", null }))
            {
                var thi = readAsset.Invoke(contentReader, null);
                return (T)thi;
            }
        }
        public static void LoadModels()
        {
            Load();

        }
    }
}