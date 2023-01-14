
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core
{
    public class ParticulateHost : IComponent
    {
        public HashSet<ParticulateField> Particulates = new HashSet<ParticulateField>();
        public void GenerateMistField(Vector2 Position, int X, int Y) => Particulates.Add(new ParticulateField(Position, X, Y));

        public void Update()
        {
            foreach (ParticulateField mf in Particulates.ToList())
            {
                mf.Update();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (ParticulateField mf in Particulates.ToList())
            {
                mf.Draw(sb);
            }
        }
    }
}