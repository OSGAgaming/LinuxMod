using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics.Interfaces
{
    public interface IDraw
    {
        void Draw(SpriteBatch spritebatch);
    }

    public interface ILayeredDraw : IDraw
    {
        string Layer { get; }
    }
}