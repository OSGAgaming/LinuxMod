using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class LiquidHost
    {
        public List<Liquid> Liquids = new List<Liquid>();

        public void AddLiquid(Rectangle Bounds, int accuracy = 100, float viscosity = 0.09f, float dampening = 0.05f)
        {
            Liquid liquid = new Liquid
            {
                frame = Bounds,
                accuracy = accuracy,
                viscosity = viscosity,
                constant = 50,
                dampening = dampening
            };

            liquid.Initialize();

            Liquids.Add(liquid);
        }

        public void UpdateLiquids()
        {
            foreach (Liquid liquid in Liquids.ToArray())
            {
                liquid.Update();
            }
        }

        public void DrawLiquids(SpriteBatch sb)
        {
            foreach(Liquid liquid in Liquids.ToArray())
            {
                liquid.Draw(sb);
            }
        }
    }
}


