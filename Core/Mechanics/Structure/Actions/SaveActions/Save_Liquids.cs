using LinuxMod.Core.Assets;
using LinuxMod.Core.Helper.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace LinuxMod.Core.Mechanics
{
    public class SaveLiquid : ISerializeWorldSpace
    {
        public void Read(BinaryReader reader, int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);

            byte liquidammout = reader.ReadByte();
            tile.liquid = liquidammout;

            if (liquidammout > 0)
            {
                byte liquidtype = reader.ReadByte();
                tile.liquidType(liquidtype);

                WorldGen.SquareTileFrame(x, y, true);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.sendWater(x, y);
                }
            }
        }

        public void Write(BinaryWriter writer, int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);

            writer.Write(tile.liquid);
            if (tile.liquid > 0) writer.Write(tile.liquidType());
        }
    }
}