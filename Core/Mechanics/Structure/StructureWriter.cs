using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

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
                    writer.Write(i);
                    writer.Write(j);

                    SaveSingleTiles SingleTiles = new SaveSingleTiles();
                    SingleTiles.Write(writer, p.X + i, p.Y + j);
                }
            }

            for (int i = 0; i < wh.X; i++)
            {
                for (int j = 0; j < wh.Y; j++)
                {
                    writer.Write(i);
                    writer.Write(j);

                    SaveTODTiles TOD = new SaveTODTiles();
                    TOD.Write(writer, p.X + i, p.Y + j);

                    SaveLiquid Liquid = new SaveLiquid();
                    Liquid.Write(writer, p.X + i, p.Y + j);
                }
            }
        }
    }
}
