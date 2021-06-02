
using LinuxMod.Core.Assets;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace LinuxMod.Core
{
	internal class DimensionUIElement : UIImage
	{
		public int DWidth;
		public int DHeight;
        public Texture2D tex;
		public DimensionUIElement(Texture2D tex) : base(tex)
        {
            this.tex = tex;
        }

        public void SetW(int w)
        {
            Width.Set(w, 0);
            DWidth = w;
        }

        public void SetH(int h)
        {
            Height.Set(h, 0);
            DHeight = h;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Point p = GetDimensions().Position().ToPoint();
            spriteBatch.Draw(tex, new Rectangle(p.X, p.Y, DWidth, DHeight), Color.White);
        }
    }
}