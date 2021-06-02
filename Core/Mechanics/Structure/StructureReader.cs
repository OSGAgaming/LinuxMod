using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class StructureReader
    {
        Stream stream { get; set; }

        public StructureReader(Stream stream)
        {
            this.stream = stream;
        }

        public void Read(Point p)
        {
            BinaryReader reader = new BinaryReader(stream);

            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    PlaceSpace(p.X, p.Y);
                }
            }
        }

        public void PlaceSpace(int x, int y)
        {
            BinaryReader reader = new BinaryReader(stream);

            bool active = reader.ReadBoolean();
            int a = reader.ReadInt32();
            int b = reader.ReadInt32();
            Tile tile = Framing.GetTileSafely(x + a, y + b);

            tile.active(active);
            if (active)
            {
                int type = reader.ReadInt32();

                if (WorldGen.InWorld(x + a, y + b, 10))
                {
                    byte slope = reader.ReadByte();
                    byte color = reader.ReadByte();

                    int frameX = reader.ReadInt32();
                    int frameY = reader.ReadInt32();

                    bool actuator = reader.ReadBoolean();

                    tile.type = (ushort)type;

                    tile.slope(slope);
                    tile.color(color);
                    tile.frameX = (short)frameX;
                    tile.frameY = (short)frameY;

                    tile.actuator(actuator);
                    WorldGen.SquareTileFrame(x + a, y + b);
                }
            }

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

            byte liquidammout = reader.ReadByte();
            tile.liquid = liquidammout;

            if (liquidammout > 0)
            {
                byte liquidtype = reader.ReadByte();
                tile.liquidType(liquidtype);

                WorldGen.SquareTileFrame(x + a, y + b, true);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.sendWater(x + a, y + b);
                }
            }

        }
    }
}
