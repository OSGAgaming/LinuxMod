using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using LinuxMod.Core.Helper;
using Terraria.ModLoader;
using LinuxMod.Core;
using LinuxMod.Core.Assets;
using Terraria.ID;
using LinuxMod.Core.Mechanics.Verlet;

namespace LinuxMod.Content.NPCs.Cutscene
{
    public class GloopMetaBall : Entity
    {
        public Vector2 dimensions;
        public Vector2 origDims;
        //to be used 
        public Texture2D tex;
        public Color color;
        public float SHMFactor;
        public float Dampening;
        public float SquishOsc;

        public GloopMetaBall(Vector2 dimensions, Texture2D tex = null)
        {
            this.dimensions = dimensions;
            origDims = dimensions;
        }

        public void Draw(SpriteBatch sb)
        {
            LinuxTechTips.DrawCircle(position.ForDraw(), dimensions, color);
        }

        public void Update()
        {
            position += velocity;
        }
    }
}
