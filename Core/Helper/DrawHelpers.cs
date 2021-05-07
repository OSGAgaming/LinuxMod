using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using LinuxMod.Core.Assets;

namespace LinuxMod.Core
{
    public static partial class LUtils
    {
        public static Vector2 DeltaScreen => Main.screenPosition - Main.screenLastPosition;
        public static void Draw(Texture2D tex, Vector2 position, Color colour, float scale, Rectangle frame = default)
        {
            Main.spriteBatch.Draw(tex, position, frame == default ? tex.Bounds : frame, colour, 0f, frame == default ? tex.TextureCenter() : frame.Center(), scale, SpriteEffects.None, 0f);
        }
        public static void Draw(Texture2D tex, Vector2 position, Color colour, float scale, Rectangle frame = default, float rotation = 0f)
        {
            Main.spriteBatch.Draw(tex, position, frame == default ? tex.Bounds : frame, colour, rotation, frame == default ? tex.TextureCenter() : frame.Center(), scale, SpriteEffects.None, 0f);
        }

        public static Texture2D RadialMask => Asset.GetTexture("Masks/RadialGradient");

        public static Vector2 TextureCenter(this Texture2D texture) => new Vector2(texture.Width / 2, texture.Height / 2);

        public static Vector2 Size(this Texture2D texture) => new Vector2(texture.Width, texture.Height);

        public static void DrawTileGlowmask(Texture2D texture, int i, int j)
        {
            int frameX = Framing.GetTileSafely(i, j).frameX;
            int frameY = Framing.GetTileSafely(i, j).frameY;

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;
            Rectangle rect = new Rectangle(frameX, frameY, 16, 16);

            Main.spriteBatch.Draw(texture, position, rect, Color.White, 0f, default, 1f, SpriteEffects.None, 0f);
        }

        public static void DrawTileGlowmask(Texture2D texture, int i, int j, Color color)
        {
            int frameX = Framing.GetTileSafely(i, j).frameX;
            int frameY = Framing.GetTileSafely(i, j).frameY;

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;
            Rectangle rect = new Rectangle(frameX, frameY, 16, 16);

            Main.spriteBatch.Draw(texture, position, rect, color, 0f, default, 1f, SpriteEffects.None, 0f);
        }

        public static Vector2 GetTileDrawZero()
        {
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            return zero;
        }
        public static void DrawBoxFill(Vector2 pos, int width, int height, Color tint) => Main.spriteBatch.Draw(Main.magicPixel, pos, new Rectangle(0, 0, width, height), tint);
        public static void DrawBoxFill(Rectangle rectangle, Color tint) => Main.spriteBatch.Draw(Main.magicPixel, rectangle.Location.ToVector2(), new Rectangle(0, 0, rectangle.Width, rectangle.Height), tint);

        public static void DrawSquare(Vector2 point, float size, Color color)
        {
            DrawLine(new Vector2(point.X + size, point.Y + size), new Vector2(point.X, point.Y + size), color);
            DrawLine(new Vector2(point.X + size, point.Y + size), new Vector2(point.X + size, point.Y), color);
            DrawLine(point, new Vector2(point.X, point.Y + size), color);
            DrawLine(point, new Vector2(point.X + size, point.Y), color);
        }

        public static void DrawRectangle(Vector2 point, float sizeX, float sizeY, Color color, float thickness = 1)
        {
            DrawLine(new Vector2(point.X + sizeX, point.Y + sizeY), new Vector2(point.X, point.Y + sizeY), color, thickness);
            DrawLine(new Vector2(point.X + sizeX, point.Y + sizeY), new Vector2(point.X + sizeX, point.Y), color, thickness);
            DrawLine(point, new Vector2(point.X, point.Y + sizeY), color, thickness);
            DrawLine(point, new Vector2(point.X + sizeX, point.Y), color, thickness);
        }
        public static void DrawRectangle(Rectangle rectangle, Color color, float thickness = 1)
        {
            Vector2 point = rectangle.Location.ToVector2();
            int sizeX = (int)rectangle.Size().X;
            int sizeY = (int)rectangle.Size().Y;
            DrawLine(new Vector2(point.X + sizeX, point.Y + sizeY), new Vector2(point.X, point.Y + sizeY), color, thickness);
            DrawLine(new Vector2(point.X + sizeX, point.Y + sizeY), new Vector2(point.X + sizeX, point.Y), color, thickness);
            DrawLine(point, new Vector2(point.X, point.Y + sizeY), color, thickness);
            DrawLine(point, new Vector2(point.X + sizeX, point.Y), color, thickness);
        }
    }
}
