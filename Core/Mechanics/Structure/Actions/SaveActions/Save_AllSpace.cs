using LinuxMod.Core.Assets;
using LinuxMod.Core.Helper.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace LinuxMod.Core.Mechanics
{
    public class SaveAllSpace : ISerializeWorldSpace
    {
        SaveSingleTiles step1 { get; set; } = new SaveSingleTiles();
        SaveTODTiles step2 { get; set; } = new SaveTODTiles();
        SaveLiquid step3 { get; set; } = new SaveLiquid();

        public void Read(BinaryReader reader, int x, int y)
        {
            step1.Read(reader, x, y);
            step2.Read(reader, x, y);
            step3.Read(reader, x, y);
        }

        public void Write(BinaryWriter writer, int x, int y)
        {
            step1.Write(writer, x, y);
            step2.Write(writer, x, y);
            step3.Write(writer, x, y);
        }
    }
}