using EEMod.Extensions;
using LinuxMod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core
{
    public static partial class Utils
    {
        internal const string EmptyTexture = "EEMod/Empty";
        // public static InteritosGlobalNPC Interitos(this NPC npc) => npc.GetGlobalNPC<InteritosGlobalNPC>();
        // public static InteritosGlobalProjectile Interitos(this Projectile proj) => proj.GetGlobalProjectile<InteritosGlobalProjectile>();

        public const BindingFlags FlagsInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        public const BindingFlags FlagsStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        public const BindingFlags FlagsALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public const float DegreeInRadians = (float)(Math.PI / 180);
        public const float RadianInDegrees = (float)(180 / Math.PI);

        public static float X(float t,
    float x0, float x1, float x2, float x3)
        {
            return (float)(
                x0 * (1 - t)* (1 - t)* (1 - t) +
                x1 * 3 * t * (1 - t) * (1 - t) +
                x2 * 3 * t*t * (1 - t) +
                x3 * t * t * t
            );
        }

        public static float Y(float t,
            float y0, float y1, float y2, float y3)
        {
            return (float)(
                 y0 * (1 - t)* (1 - t)* (1 - t) +
                 y1 * 3 * t * (1 - t) * (1 - t) +
                 y2 * 3 * t * t * (1 - t) +
                 y3 * t * t * t
             );
        }
        private static float X(float t,
   float x0, float x1, float x2)
        {
            return (float)(
                x0 * (1 - t) * (1 - t) +
                x1 * 2 * t * (1 - t) +
                x2 * t * t
            );
        }

        public static float Y(float t,
            float y0, float y1, float y2)
        {
            return (float)(
                y0 * (1 - t) * (1 - t) +
                y1 * 2 * t * (1 - t) +
                y2 * t * t
            );
        }
        public static int[,] ConvertTexToBitmap(string tex, int thresh)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap($@"{Main.SavePath}\Mod Sources\EEMod\" + tex + ".png");
            int[,] Array = new int[bitmap.Width, bitmap.Height];
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Array[i, j] = bitmap.GetPixel(i, j).R < thresh ? 1 : 0;
                }
            }
            return Array;
        }
        public static void TexToDust(string path, Vector2 position, int accuracy = 1, float spacing = 1, int threshold = 126)
        {
            int[,] array = ConvertTexToBitmap(path, threshold);
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j] == 1 && i % accuracy == 0 && j % accuracy == 0)
                    {
                        Dust dust = Dust.NewDustPerfect(position + new Vector2(i, j) * spacing, 219, Vector2.Zero);
                        dust.noGravity = true;
                    }
                }
            }
        }
        public static ParticleZone Particles => LinuxMod.GlobalZone.Get("Main");
        public static void DrawParticlesAlongBezier(Vector2 endPoints, Vector2 startingPos, Vector2 c1, float chainsPerUse, Color color, float spawnChance = 1f, params IParticleModule[] modules)
        {
            for (float i = 0; i <= 1; i += chainsPerUse)
            {
                if (i != 0)
                {
                    float x = X(i, startingPos.X, c1.X, endPoints.X);
                    float y = Y(i, startingPos.Y, c1.Y, endPoints.Y);

                    Particles.SetSpawningModules(new SpawnRandomly(spawnChance));
                    Particles.SpawnParticles(new Vector2(x, y), default, 2, color, modules);
                }
            }
        }
        public static void DrawParticlesAlongLine(Vector2 endPoints, Vector2 startingPos, float chainsPerUse, Color color, float spawnChance = 1f, params IParticleModule[] modules)
        {
            for (float i = 0; i <= 1; i += chainsPerUse)
            {
                if (i != 0)
                {
                    Vector2 pos = Vector2.Lerp(startingPos, endPoints,i);

                    Particles.SetSpawningModules(new SpawnRandomly(spawnChance));
                    Particles.SpawnParticles(pos, default, 2, color, modules);
                }
            }
        }
        public static void DrawParticlesAlongBezier(Vector2 endPoints, Vector2 startingPos, Vector2 c1, float chainsPerUse, Color color, float spawnChance = 1f,Vector2 velocity = default, params IParticleModule[] modules)
        {
            for (float i = 0; i <= 1; i += chainsPerUse)
            {
                if (i != 0)
                {
                    float x = X(i, startingPos.X, c1.X, endPoints.X);
                    float y = Y(i, startingPos.Y, c1.Y, endPoints.Y);

                    Particles.SetSpawningModules(new SpawnRandomly(spawnChance));
                    Particles.SpawnParticles(new Vector2(x, y), velocity, 2, color, modules);
                }
            }
        }
        public static void DrawChain(Texture2D tex, int frameSize, int frameNum, Vector2 p1, Vector2 p2, float rotOffset = 0, float per = 1, Color color = default)
        {
            //USE IN PROPER HOOK PLZ THX
            float width = tex.Width;
            float length = (p1 - p2).Length();
            float rotation = (p1 - p2).ToRotation();
            Rectangle rect = new Rectangle(0, frameNum * frameSize, (int)width, frameSize);

            for (float i = 0; i < 1; i += (width / length) * per)
            {
                Vector2 lerp = p1 + (p2 - p1) * i;
                Main.spriteBatch.Draw(tex, lerp.ForDraw(), rect, color, rotation + rotOffset, rect.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }
        public static void DrawChain(Texture2D tex, Vector2 p1, Vector2 p2, float rotOffset = 0)
        {
            //USE IN PROPER HOOK PLZ THX
            float width = tex.Width;
            float length = (p1 - p2).Length();
            float rotation = (p1 - p2).ToRotation();
            Rectangle rect = new Rectangle(0, 0, tex.Width, tex.Height);
            for (float i = 0; i < 1; i += width / length)
            {
                Vector2 lerp = p1 + (p2 - p1) * i;
                Main.spriteBatch.Draw(tex, lerp.ForDraw(), rect, Color.White, rotation + rotOffset, rect.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }
        public static void DrawChain(Texture2D tex, Vector2 p1, Vector2 p2, float rotOffset = 0,float per = 1)
        {
            //USE IN PROPER HOOK PLZ THX
            float width = tex.Width;
            float length = (p1 - p2).Length();
            float rotation = (p1 - p2).ToRotation();
            Rectangle rect = new Rectangle(0, 0, tex.Width, tex.Height);
            for (float i = 0; i < 1; i += (width / length)*per)
            {
                Vector2 lerp = p1 + (p2 - p1) * i;
                Main.spriteBatch.Draw(tex, lerp.ForDraw(), rect, Color.White, rotation + rotOffset, rect.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }
        public static void DrawChain(Texture2D tex, Vector2 p1, Vector2 p2, float rotOffset, float per, Color color)
        {
            //USE IN PROPER HOOK PLZ THX
            float width = tex.Width;
            float length = (p1 - p2).Length();
            float rotation = (p1 - p2).ToRotation();
            Rectangle rect = new Rectangle(0, 0, tex.Width, tex.Height);
            for (float i = 0; i < 1; i += (width / length) * per)
            {
                Vector2 lerp = p1 + (p2 - p1) * i;
                Main.spriteBatch.Draw(tex, lerp.ForDraw(), rect, color, rotation + rotOffset, rect.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }
        public static void DrawChain(Texture2D tex, Vector2 p1, Vector2 p2, float rotOffset = 0, float per = 1, Func<float, float> rotAct = null, Func<float, Vector2> posAct = null)
        {
            //USE IN PROPER HOOK PLZ THX
            float width = tex.Width;
            float length = (p1 - p2).Length();
            float rotation = (p1 - p2).ToRotation();
            Rectangle rect = new Rectangle(0, 0, tex.Width, tex.Height);
            for (float i = 0; i < 1; i += (width / length) * per)
            {
                Vector2 lerp = p1 + (p2 - p1) * i;
                Main.spriteBatch.Draw(tex, lerp.ForDraw() + posAct.Invoke(i), rect, Color.White, rotation + rotOffset + rotAct.Invoke(i), rect.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }
        public static void DrawChain(Texture2D tex, Vector2 p1, Vector2 p2, float rotOffset, float per, Func<float, float> rotAct, Func<float, Vector2> posAct, Color color)
        {
            //USE IN PROPER HOOK PLZ THX
            float width = tex.Width;
            float length = (p1 - p2).Length();
            float rotation = (p1 - p2).ToRotation();
            Rectangle rect = new Rectangle(0, 0, tex.Width, tex.Height);
            for (float i = 0; i < 1; i += (width / length) * per)
            {
                Vector2 lerp = p1 + (p2 - p1) * i;
                Main.spriteBatch.Draw(tex, lerp.ForDraw() + posAct.Invoke(i), rect, color, rotation + rotOffset + rotAct.Invoke(i), rect.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }
        public static void DrawChain(Texture2D tex, Vector2 p1, Vector2 p2, Rectangle rect, Color color, float rotOffset = 0, float per = 1, Func<float, float> rotAct = null, Func<float, Vector2> posAct = null)
        {
            //USE IN PROPER HOOK PLZ THX
            float width = tex.Width;
            float length = (p1 - p2).Length();
            float rotation = (p1 - p2).ToRotation();
            for (float i = 0; i < 1; i += (width / length) * per)
            {
                Vector2 lerp = p1 + (p2 - p1) * i;
                Main.spriteBatch.Draw(tex, lerp.ForDraw() + posAct.Invoke(i), rect, color, rotation + rotOffset + rotAct.Invoke(i), rect.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }
        public static Vector2 TraverseBezier(Vector2 endPoints, Vector2 startingPos, Vector2 c1, Vector2 c2, float t)
        {
            float x = X(t, startingPos.X, c1.X, c2.X, endPoints.X);
            float y = Y(t, startingPos.Y, c1.Y, c2.Y, endPoints.Y);
            return new Vector2(x, y);
        }
        public static Vector2 TraverseBezier(Vector2 endPoints, Vector2 startingPos, Vector2 c1, float t)
        {
            float x = X(t, startingPos.X, c1.X, endPoints.X);
            float y = Y(t, startingPos.Y, c1.Y, endPoints.Y);
            return new Vector2(x, y);
        }

        public static Rectangle[] ReturnPoints(Vector2 endPoints, Vector2 startingPos, Vector2 c1, Vector2 c2, float chainsPerUse, int chogsizeX, int chogsizeY, int accuracy)
        {
            Rectangle[] collision = new Rectangle[(int)(1 / (chainsPerUse * accuracy)) + 1]; //41
            int keeper = -2;
            for (float i = 0; i <= 1; i += chainsPerUse)
            {
                keeper++;
                if (i != 0 && keeper % accuracy == 0)
                {
                    collision[keeper / accuracy] = new Rectangle((int)X(i, startingPos.X, c1.X, c2.X, endPoints.X) - chogsizeX / 2, (int)Y(i, startingPos.Y, c1.Y, c2.Y, endPoints.Y) - chogsizeY / 2, chogsizeX, chogsizeY);
                }
            }
            return collision;
        }
        public static void NewTextAutoSync(string text, Color color)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, color);
            }
            else
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), color);
            }
        }

        public static bool PlayerIsInForest(Player player)
        {
            return !player.ZoneJungle
                && !player.ZoneDungeon
                && !player.ZoneCorrupt
                && !player.ZoneCrimson
                && !player.ZoneHoly
                && !player.ZoneSnow
                && !player.ZoneUndergroundDesert
                && !player.ZoneGlowshroom
                && !player.ZoneMeteor
                && !player.ZoneBeach
                && player.ZoneOverworldHeight;
        }
        public static void DrawLine(Vector2 p1, Vector2 p2, Color tint = default, float lineWidth = 1f)
        {
            Vector2 between = p2 - p1;
            float length = between.Length();
            float rotation = (float)Math.Atan2(between.Y, between.X);
            Main.spriteBatch.Draw(Main.magicPixel, p1, new Rectangle(0, 0, 1, 1), tint == default ? Color.White : tint, rotation, new Vector2(0f, 0.5f), new Vector2(length, lineWidth), SpriteEffects.None, 0f);
        }
        public static void SpawnOre(int type, double frequency, float depth, float depthLimit)
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int k = 0; k < (int)(x * y * frequency); k++)
                {
                    int tilesX = WorldGen.genRand.Next(0, x);
                    int tilesY = WorldGen.genRand.Next((int)(y * depth), (int)(y * depthLimit));
                    WorldGen.OreRunner(tilesX, tilesY, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                }
            }
        }

        /* public static class VanillasDoing
        {
            public static void DrawNPCVanilla()
            {
            }
        } */

        public static class Debug
        {
            public static void LogChat(string obj, [CallerFilePath] string callerName = "", [CallerLineNumber] int lineNumber = -1)
            {
                Main.NewText(obj + $"<{Path.GetDirectoryName(callerName).Split('\\').Last()}/{Path.GetFileName(callerName)}>" + $"({lineNumber})", 0, 255, 0);
            }

            public static void Log(object obj, [CallerFilePath] string callerName = "", [CallerLineNumber] int lineNumber = -1)
            {
                LogChat(obj.ToString(), callerName, lineNumber);
            }

            public static void LogError(string obj, [CallerFilePath] string callerName = "", [CallerLineNumber] int lineNumber = -1)
            {
                Main.NewText(obj + $"<{Path.GetDirectoryName(callerName).Split('\\').Last()}/{Path.GetFileName(callerName)}>" + $"({lineNumber})", 255, 0, 0);
            }

            public static void Message(string obj, Color color = default)
            {
                if (color == default)
                {
                    color = Color.White;
                }

                Main.NewText(obj, color);
            }
        }
    }
}
