
using LinuxMod.Core.Assets;
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;

namespace LinuxMod.Core.Mechanics.Primitives
{
    internal class WaterPrimtives : Primitive
    {
        private Liquid water;
        public WaterPrimtives(Liquid water)
        {
            this.water = water;
        }
        public override void SetDefaults()
        {
            _alphaValue = 0.4f;
            _width = 1;
            _cap = 1000;
        }
        public override void PrimStructure(SpriteBatch spriteBatch)
        {
            Color colour = water.color*_alphaValue;

            for (int i = 0; i < _points.Count - 1; i++)
            {
                AddVertex(_points[i] - LUtils.DeltaScreen, colour, new Vector2(i / (float)(_points.Count), 0));
                AddVertex(new Vector2(_points[i + 1].X, water.frame.Bottom) - LUtils.DeltaScreen, colour, new Vector2((i + 1) / (float)(_points.Count), 1));
                AddVertex(new Vector2(_points[i].X, water.frame.Bottom) - LUtils.DeltaScreen, colour, new Vector2(i / (float)(_points.Count), 1));

                AddVertex(_points[i] - LUtils.DeltaScreen, colour, new Vector2(i / (float)(_points.Count), 0));
                AddVertex(_points[i + 1] - LUtils.DeltaScreen, colour, new Vector2((i + 1) / (float)(_points.Count), 0));
                AddVertex(new Vector2(_points[i + 1].X, water.frame.Bottom) - LUtils.DeltaScreen, colour, new Vector2((i + 1) / (float)(_points.Count), 1));
            }
        }
        public override void SetShaders()
        {
            Effect effect = LinuxMod.PrimitiveShaders;
            effect.Parameters["dimensions"].SetValue(water.frame.Size());
            effect.Parameters["averageDimensions"].SetValue(new Vector2(100,20));
            PrepareShader(effect, "Example", _counter);
        }
       
        public override void OnUpdate()
        {
            ScreenMapPass.Instance.GetMap("CutsceneWaterReflection").DrawToTarget(Draw);

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