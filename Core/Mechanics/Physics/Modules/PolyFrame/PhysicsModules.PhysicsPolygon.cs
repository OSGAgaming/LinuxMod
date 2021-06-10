using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Verlet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class PolygonModule : Module
    {
        public Polygon Polygon;
        public override void Draw(SpriteBatch sb)
        {
            new Polygon(Polygon.points, Object.Center).Draw();
        }
        public PolygonModule(Polygon polygon)
        {
            Polygon = polygon;
        }

        public PolygonModule(Rectangle rectangle)
        {
            Polygon = Polygon.RectToPoly(rectangle);
        }
    }
}