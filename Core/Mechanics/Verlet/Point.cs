using EEMod.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core.Mechanics.Verlet
{
    public class Point
    {
        public Vector2 point;
        public Vector2 oldPoint;
        public Vector2 vel;
        public bool isStatic;

        public Point(Vector2 point, Vector2 oldPoint, bool isStatic)
        {
            this.point = point;
            this.oldPoint = oldPoint;
            this.isStatic = isStatic;
            vel = Vector2.Zero;
        }

    }

}
