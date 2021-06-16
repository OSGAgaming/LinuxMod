using LinuxMod.Content.Tiles;
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
                    int a = reader.ReadInt32();
                    int b = reader.ReadInt32();

                    SaveSingleTiles Reader = new SaveSingleTiles();
                    Reader.Read(reader, p.X + a, p.Y + b);
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int a = reader.ReadInt32();
                    int b = reader.ReadInt32();

                    SaveTODTiles Reader = new SaveTODTiles();
                    Reader.Read(reader, p.X + a, p.Y + b);

                    SaveLiquid Liquid = new SaveLiquid();
                    Liquid.Read(reader, p.X + a, p.Y + b);
                }
            }
        }
    }
}
