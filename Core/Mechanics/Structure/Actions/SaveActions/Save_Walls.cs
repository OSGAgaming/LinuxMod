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
    public class SaveWalls : ISerializeWorldSpace
    {
        public void Read(BinaryReader reader, int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            int wallType = reader.ReadInt32();

            if (wallType > 0)
            {
                byte wallColor = reader.ReadByte();
                int wallFrameX = reader.ReadInt32();
                int wallFrameY = reader.ReadInt32();

                tile.wall = (ushort)wallType;
                tile.wallColor(wallColor);
                tile.wallFrameX(wallFrameX);
                tile.wallFrameY(wallFrameY);
            }
        }

        public void Write(BinaryWriter writer, int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            writer.Write((int)tile.wall);

            if (tile.wall > 0)
            {
                writer.Write(tile.wallColor());
                writer.Write(tile.wallFrameX());
                writer.Write(tile.wallFrameY());
            }
        }
    }
}