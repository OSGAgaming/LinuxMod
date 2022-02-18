using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinuxMod.Core.Mechanics.Primitives
{
    public class QuadMesh : IndexedPrimitive
    {
        public ref VertexPositionColorTexture TopLeft => ref vertices[0];

        public ref VertexPositionColorTexture TopRight => ref vertices[1];

        public ref VertexPositionColorTexture BottomRight => ref vertices[2];

        public ref VertexPositionColorTexture BottomLeft => ref vertices[3];

        public QuadMesh(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color, string layer = "Default", Texture2D texture = null, Effect effect = null) : base(4, 6, layer, texture, effect)
        {
            Construct(v1, v2, v3, v4, color);
        }

        public void Construct(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color)
        {
            AddVertex(v1, color, Vector2.Zero);
            AddVertex(v2, color, Vector2.UnitX);
            AddVertex(v3, color, Vector2.One);
            AddVertex(v4, color, Vector2.UnitY);

            AddIndex(0);
            AddIndex(1);
            AddIndex(2);

            AddIndex(0);
            AddIndex(3);
            AddIndex(2);

            Finish();
        }

        public IEnumerable<VertexPositionNormalTexture> ConvertToNormal()
        {
            for (int i = 0; i < vertexPointer; i++)
                yield return new VertexPositionNormalTexture(vertices[i].Position, Vector3.Backward, vertices[i].TextureCoordinate);
        }
    }
}
