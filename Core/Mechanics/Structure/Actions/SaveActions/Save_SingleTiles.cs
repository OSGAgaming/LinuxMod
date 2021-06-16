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
    public class SaveSingleTiles : ISerializeWorldSpace
    {
        public void Read(BinaryReader reader, int x, int y)
        {
            bool active = reader.ReadBoolean();

            bool NoTOD = reader.ReadBoolean();

            Tile tile = Framing.GetTileSafely(x, y);

            int frameX = reader.ReadInt32();
            int frameY = reader.ReadInt32();

            int type = reader.ReadInt32();
            tile.active(active);

            if (active && NoTOD)
            {
                byte slope = reader.ReadByte();
                byte color = reader.ReadByte();

                bool actuator = reader.ReadBoolean();

                tile.type = (byte)type;
                tile.frameX = (short)frameX;
                tile.frameY = (short)frameY;
                tile.slope(slope);
                tile.color(color);

                tile.actuator(actuator);

                WorldGen.SquareTileFrame(x, y);
            }
        }


        public void Write(BinaryWriter writer, int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);

            writer.Write(tile.active());

            TileObjectData TOD = TileObjectData.GetTileData(tile.type, 0);

            writer.Write(TOD == null);

            writer.Write((int)tile.frameX);
            writer.Write((int)tile.frameY);

            writer.Write((int)tile.type);

            if (tile.active() && TOD == null)
            {
                writer.Write(tile.slope());
                writer.Write(tile.color());

                writer.Write(tile.actuator());
            }
        }
    }
}