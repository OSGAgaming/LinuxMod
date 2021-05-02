using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.World.Generation;
using Terraria.ModLoader.IO;

namespace Starjinx
{
    public class LinuxWorld : ModWorld
    {
        public static bool Excecutable = false;

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("Excecutable"))
            {
                Excecutable = tag.GetBool("Excecutable");
            }
        }
        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();
            tag["Excecutable"] = Excecutable;
            return tag;
        }
   
    }
}