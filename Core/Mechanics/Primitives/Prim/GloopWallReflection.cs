
using LinuxMod.Core.Assets;
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;

namespace LinuxMod.Core.Mechanics.Primitives
{
    internal class GloopWallReflection : Primitive
    {
        private Liquid water;
        private int Height;
        public GloopWallReflection(Liquid water)
        {
            this.water = water;
        }
        public override void SetDefaults()
        {
            _alphaValue = 0.4f;
            _width = 1;
            _cap = 1000;
            Height = 70;
        }
        public override void PrimStructure(SpriteBatch spriteBatch)
        {
            Color colour = Color.White*_alphaValue;

            for (int i = 0; i < _points.Count - 1; i++)
            {
                Vector2 v = new Vector2(_points[i].X, water.frame.Y - Height);
                Vector2 v1 = new Vector2(_points[i + 1].X, water.frame.Y - Height);

                AddVertex(v, colour, new Vector2(i / (float)(_points.Count), 0));
                AddVertex(new Vector2(_points[i + 1].X, _points[i + 1].Y), colour, new Vector2((i + 1) / (float)(_points.Count), 1));
                AddVertex(new Vector2(_points[i].X, _points[i].Y), colour, new Vector2(i / (float)(_points.Count), 1));

                AddVertex(v, colour, new Vector2(i / (float)(_points.Count), 0));
                AddVertex(v1, colour, new Vector2((i + 1) / (float)(_points.Count), 0));
                AddVertex(new Vector2(_points[i + 1].X, _points[i + 1].Y), colour, new Vector2((i + 1) / (float)(_points.Count), 1));
            }
        }
        public override void SetShaders()
        {
            Effect effect = LinuxMod.PrimitiveShaders;
            effect.Parameters["dimensions"].SetValue(water.frame.Size());
            effect.Parameters["averageDimensions"].SetValue(new Vector2(100,20));
            PrepareShader(effect, "WReflect", _counter);
        }
       
        public override void OnUpdate()
        {
            ScreenMapPass.Instance.GetMap("WaterWall").DrawToBatchedTarget(Draw);

            _points = water.Pos.ToList();
            _counter++;
            _noOfPoints = _points.Count() * 6;
            if (_cap < _noOfPoints / 6)
            {
                _points.RemoveAt(0);
            }
            if (water == null)
            {
                Dispose();
            }
        }
        public override void OnDestroy()
        {
            _width *= 0.9f;
            if (_width < 0.05f)
            {
                Dispose();
            }
        }
    }
}