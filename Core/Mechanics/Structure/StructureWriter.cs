using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class StructureWriter
    {
        Stream stream { get; set; }

        public StructureWriter(Stream stream)
        {
            this.stream = stream;
        }

        public void Write(Point p, Point wh)
        {
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(wh.X);
            writer.Write(wh.Y);

            for (int i = 0; i < wh.X; i++)
            {
                for (int j = 0; j < wh.Y; j++)
                {
                    WriteTileSpace(p, i, j);
                }
            }
        }

        public void WriteTileSpace(Point p, int x, int y)
        {
            Tile tile = Framing.GetTileSafely(p.X + x, p.Y + y);
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(tile.active());
            writer.Write(x);
            writer.Write(y);

            if (tile.active())
            {
                writer.Write((int)tile.type);

                writer.Write(tile.slope());
                writer.Write(tile.color());

                writer.Write((int)tile.frameX);
                writer.Write((int)tile.frameY);

                writer.Write(tile.actuator());
            }

            writer.Write((int)tile.wall);

            if (tile.wall > 0)
            {
                writer.Write(tile.wallColor());
                writer.Write(tile.wallFrameX());
                writer.Write(tile.wallFrameY());
            }

            writer.Write(tile.liquid);
            if(tile.liquid > 0) writer.Write(tile.liquidType());
        }
    }
}
