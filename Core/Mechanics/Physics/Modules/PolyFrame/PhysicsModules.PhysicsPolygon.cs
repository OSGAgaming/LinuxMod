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
            Polygon = new Polygon(Polygon.points, Object.Center);
            Polygon.Draw();
        }
        public PolygonModule(Polygon polygon)
        {
            Polygon = polygon;
            GetHost<PolygonModule>().AddObject(this);
        }

        public PolygonModule(Rectangle rectangle)
        {
            Polygon = Polygon.RectToPoly(rectangle);
            GetHost<PolygonModule>().AddObject(this);
        }
    }
}