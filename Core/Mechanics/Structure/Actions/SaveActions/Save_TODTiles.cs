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
    public class SaveTODTiles : ISerializeWorldSpace
    {
        public void Read(BinaryReader reader, int x, int y)
        {
            bool active = reader.ReadBoolean();

            bool NoTOD = reader.ReadBoolean();

            int frameX = reader.ReadInt32();
            int frameY = reader.ReadInt32();

            int type = reader.ReadInt32();

            if (active && !NoTOD)
            {
                int OrginX = reader.ReadInt32();
                int OrginY = reader.ReadInt32();

                if (OrginY == frameY / 18)
                {
                    WorldGen.PlaceTile(x, y, type, false, true);
                }
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

            if (tile.active() && TOD != null)
            {
                Main.NewText(tile.blockType());
                Point16 Origin = TOD.Origin;

                writer.Write((int)Origin.X);
                writer.Write((int)Origin.Y);
            }
        }
    }
}