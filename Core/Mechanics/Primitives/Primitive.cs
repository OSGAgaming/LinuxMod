
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using EEMod.Extensions;
using System.Linq;
using System;
using static Terraria.ModLoader.ModContent;
using System.Reflection;
namespace LinuxMod.Core.Mechanics.Primitives
{
    public class PrimitiveManager
    {
        public List<Primitive> _trails = new List<Primitive>();
        public void Draw(SpriteBatch sb)
        {
            foreach (Primitive trail in _trails.ToArray())
            {
                trail.Draw(sb);
            }
        }
        public void Update()
        {
            foreach (Primitive trail in _trails.ToArray())
            {
                trail.Update();
            }
        }
        public void CreateTrail(Primitive PT) => _trails.Add(PT);
    }
    public partial class Primitive
    {
        protected float _width;
        protected float _alphaValue;
        protected int _cap;
        protected ITrailShader _trailShader;
        protected int _counter;
        protected int _noOfPoints;
        protected List<Vector2> _points = new List<Vector2>();
        protected bool _destroyed = false;

        protected GraphicsDevice _device;
        protected Effect _effect;
        protected BasicEffect _basicEffect;
        protected VertexPositionColorTexture[] vertices;
        protected int currentIndex;

        public Primitive()
        {
            _trailShader = new DefaultShader();
            _device = Main.graphics.GraphicsDevice;
            _basicEffect = new BasicEffect(_device);
            _basicEffect.VertexColorEnabled = true;
            SetDefaults();
            vertices = new VertexPositionColorTexture[_cap];
        }


        public void Dispose()
        {
            PrimitivePass.Instance.Primitives._trails.Remove(this);
        }
        public void Update()
        {
            OnUpdate();
        }
        public void Draw(SpriteBatch sb)
        {
            vertices = new VertexPositionColorTexture[_noOfPoints];
            currentIndex = 0;

            PrimStructure(sb);
            SetShaders();
            if (_noOfPoints >= 1)
                _device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, _noOfPoints / 3);
        }
        public virtual void OnUpdate() { }
        public virtual void PrimStructure(SpriteBatch spriteBatch) { }
        public virtual void SetShaders() { }
        public virtual void SetDefaults() { }
        public virtual void OnDestroy() { }
        //Helper methods
    }
}