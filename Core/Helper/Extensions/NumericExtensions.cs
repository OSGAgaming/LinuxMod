using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace LinuxMod.Core.Helper.Extensions
{
    public static class NumericExtensions
    {
        public static Vector2 ParalaxX(this Vector2 vec, float paralax) => vec - new Vector2(Main.LocalPlayer.Center.X * paralax, Main.LocalPlayer.Center.Y * paralax);

        public static Vector2 ParalaxXY(this Vector2 vec, Vector2 paralax) => vec - new Vector2(Main.LocalPlayer.Center.X * paralax.X, Main.LocalPlayer.Center.Y * paralax.Y);
        public static Vector2 ForDraw(this Vector2 vec) => vec - Main.screenPosition;

        public static float ToRadians(this float num) => num * (float)(Math.PI / 180f);

        public static float ToRadians(this double num) => (float)(num * (Math.PI / 180f));

        public static float ToRadians(this int num) => num * (float)(Math.PI / 180f);

        public static float PositiveSin(this float num) => (num / 2f) + 0.5f;

        public static float PositiveSin(this double num) => (float)(num / 2f) + 0.5f;

        public static int RoundTo(this float num, int r) => (int)(num/r)*r;

        public static Vector2 RoundTo(this Vector2 num, int r) => new Vector2(num.X.RoundTo(r), num.Y.RoundTo(r));
    }
}