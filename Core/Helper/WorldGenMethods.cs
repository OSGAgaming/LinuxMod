
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using System.Diagnostics;
//using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Terraria.DataStructures;

using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;

namespace LinuxMod.Core
{
    public enum TileSpacing
    {
        None,
        Bottom,
        Top,
        Right,
        Left
    }
    public static partial class LinuxTechTips
    {
        public static void FillRegionNoEdit(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    tile.type = (ushort)type;
                    tile.active(true);
                }
            }
        }

        public static void FillWall(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.PlaceWall(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                }
            }
        }

        public static void FillRegionDiag(int width, int height, Vector2 startingPoint, int type, int leftOrRight)
        {
            if (leftOrRight == 0)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height - i; j++)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                    }
                }
            }
            if (leftOrRight == 1)
            {
                for (int i = width; i > -1; i--)
                {
                    for (int j = 0; j < i; j++)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X - width, j + (int)startingPoint.Y, type);
                    }
                }
            }
        }


        private static void Hole(int height, int width, Vector2 startingPoint)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    WorldGen.KillTile((int)startingPoint.X + j, (int)startingPoint.Y + i);
                    WorldGen.KillWall((int)startingPoint.X + j, (int)startingPoint.Y + i);
                }
            }
        }

        public static void ClearOval(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                    {
                        WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }

                    if (i == width / 2 && j == height / 2)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y + 2, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(10, 20), TileID.StoneSlab, true, 0f, 0f, true, true);
                    }
                }
            }
        }

        public static void MakeLavaPit(int width, int height, Vector2 startingPoint, float lavaLevel)
        {
            ClearOval(width, height, startingPoint);
            FillRegionWithLava(width, (int)(height * lavaLevel), new Vector2(startingPoint.X, startingPoint.Y + (int)(height - (height * lavaLevel))));
        }

        public static void GenerateStructure(int i, int j, int[,] shape, int[] blocks, int[] paints = null, int[,] wallShape = null, int[] walls = null, int[] wallPaints = null)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        tile.type = (ushort)blocks[shape[y, x]];
                        if (paints[blocks[shape[y, x]]] != default)
                        {
                            tile.color((byte)paints[blocks[shape[y, x]]]);
                        }

                        tile.active(true);
                    }
                }
            }
            if (wallShape != null && walls != null)
            {
                for (int y = 0; y < wallShape.GetLength(0); y++)
                {
                    for (int x = 0; x < wallShape.GetLength(1); x++)
                    {
                        int k = i - 3 + x;
                        int l = j - 6 + y;
                        if (WorldGen.InWorld(k, l, 30))
                        {
                            Tile tile = Framing.GetTileSafely(k, l);
                            tile.wall = (ushort)walls[wallShape[y, x]];
                            if (wallPaints[walls[wallShape[y, x]]] != default)
                            {
                                tile.color((byte)wallPaints[walls[wallShape[y, x]]]);
                            }
                        }
                    }
                }
            }
        }


        public static void ClearRegion(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (WorldGen.InWorld(i + (int)startingPoint.X, j + (int)startingPoint.Y, 2))
                    {
                        Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                        tile.ClearTile();
                        WorldGen.KillWall(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }
                }
            }
        }
        public static Vector2 FindClosest(Vector2 pos, Vector2[] List)
        {
            Vector2 closest = Vector2.Zero;
            for (int i = 0; i < List.Length; i++)
            {
                if (closest == Vector2.Zero || Vector2.DistanceSquared(pos, List[i]) < Vector2.DistanceSquared(pos, closest) && Vector2.DistanceSquared(pos, List[i]) > 5)
                {
                    closest = List[i];
                }
            }
            return closest;
        }
        public static Vector2[] MakeDistantLocations(int number, float distance, Rectangle Bounds, int maxIterations = 100)
        {
            List<Vector2> Points = new List<Vector2>();
            for (int k = 0; k < number; k++)
            {
                Vector2 chosen = Vector2.Zero;
                if (Points.Count != 0)
                {
                    int count = -1;
                    int iterations = 0;

                    while ((count == -1 || count != 0) && iterations < maxIterations)
                    {
                        chosen = new Vector2(WorldGen.genRand.NextFloat(Bounds.Left, Bounds.Right), WorldGen.genRand.NextFloat(Bounds.Top, Bounds.Bottom));
                        count = 0;
                        for (int i = 0; i < Points.Count; i++)
                        {
                            if (Vector2.DistanceSquared(chosen, Points[i]) < distance * distance)
                            {
                                count++;
                            }
                        }
                        iterations++;
                    }
                    Points.Add(chosen);
                }
                else
                {
                    Points.Add(Bounds.Center.ToVector2());
                }
            }
            return Points.ToArray();
        }
        public static void ClearRegionSafely(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).type == type)
                    {
                        WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }
                }
            }
        }

        public static void FillRegionWithWater(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).liquidType(0); // set liquid type 0 is water 1 lava 2 honey 3+ water iirc
                    Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).liquid = 255; // set liquid ammount
                    WorldGen.SquareTileFrame(i + (int)startingPoint.X, j + (int)startingPoint.Y, true); // soemthing for astatic voiding the liquid from being static
                    if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                    {
                        NetMessage.sendWater(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }
                }
            }
        }

        public static void FillRegionWithLava(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (WorldGen.InWorld(i + (int)startingPoint.X, j + (int)startingPoint.Y))
                    {
                        Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).liquidType(1); // set liquid type 0 is water 1 lava 2 honey 3+ water iirc
                        Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).liquid = 255; // set liquid ammount
                        WorldGen.SquareTileFrame(i + (int)startingPoint.X, j + (int)startingPoint.Y, true); // soemthing for astatic voiding the liquid from being static
                        if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                        {
                            NetMessage.sendWater(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                        }
                    }
                }
            }
        }
        public static void RemoveWaterFromRegion(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).liquidType() == 0 && Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).liquid > 64)
                    {
                        Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).ClearEverything();
                        if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                        {
                            NetMessage.sendWater(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                        }
                    }
                }
            }
        }

        public static int TileCheck2(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j - 1);
            Tile tileBelow2 = Framing.GetTileSafely(i, j - 2);
            Tile tileAbove = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove2 = Framing.GetTileSafely(i, j + 2);
            Tile TileLeft = Framing.GetTileSafely(i - 1, j);
            Tile tileLeft2 = Framing.GetTileSafely(i - 2, j);
            Tile tileRight = Framing.GetTileSafely(i + 1, j);
            Tile tileRight2 = Framing.GetTileSafely(i + 2, j);
            if (tile.active() && tileBelow.active() && tileBelow2.active() && !tileAbove.active() && !tileAbove2.active() && tile.slope() == 0)
            {
                return 1;
            }
            if (tile.active() && !tileBelow.active() && !tileBelow2.active() && tileAbove.active() && tileAbove2.active() && tile.slope() == 0)
            {
                return 2;
            }
            if (tile.active() && TileLeft.active() && tileLeft2.active() && !tileRight.active() && !tileRight2.active())
            {
                return 3;
            }
            if (tile.active() && !TileLeft.active() && !tileLeft2.active() && tileRight.active() && tileRight2.active())
            {
                return 4;
            }
            else
            {
                return 0;
            }
            if (WorldGen.InWorld(i, j, 4))
            {
                Tile tile1 = Framing.GetTileSafely(i, j);
                Tile tile2 = Framing.GetTileSafely(i, j - 1);
                Tile tile3 = Framing.GetTileSafely(i, j - 2);
                Tile tile4 = Framing.GetTileSafely(i, j + 1);
                Tile tile5 = Framing.GetTileSafely(i, j + 2);
                Tile tile6 = Framing.GetTileSafely(i - 1, j);
                Tile tile7 = Framing.GetTileSafely(i - 2, j);
                Tile tile8 = Framing.GetTileSafely(i + 1, j);
                Tile tile9 = Framing.GetTileSafely(i + 2, j);

                if (tile1.active() && tile2.active() && tile3.active() && !tile4.active() && !tile5.active() && tile1.slope() == 0)
                {
                    return 1;
                }
                if (tile1.active() && !tile2.active() && !tile3.active() && tile4.active() && tile5.active() && tile1.slope() == 0)
                {
                    return 2;
                }
                if (tile1.active() && tile6.active() && tile7.active() && !tile8.active() && !tile9.active())
                {
                    return 3;
                }
                if (tile1.active() && !tile6.active() && !tile7.active() && tile8.active() && tile9.active())
                {
                    return 4;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }
        public static bool CheckRangeRight(int i, int j, int length, bool opposite = false)
        {
            for (int k = 0; k < length; k++)
            {
                if (WorldGen.InWorld(i + (opposite ? -k : k), j, 20))
                {
                    if (!Framing.GetTileSafely(i + (opposite ? -k : k), j).active() || !Main.tileSolid[Framing.GetTileSafely(i + (opposite ? -k : k), j).type])
                        return false;
                }
            }

            return true;
        }
        public static bool CheckRangeDown(int i, int j, int length, bool opposite = false)
        {
            for (int k = 0; k < length; k++)
            {
                if (WorldGen.InWorld(i, j + (opposite ? -k : k), 20))
                {
                    if (!Framing.GetTileSafely(i, j + (opposite ? -k : k)).active() || !Main.tileSolid[Framing.GetTileSafely(i, j + (opposite ? -k : k)).type])
                        return false;
                }
            }
            return true;
        }
        public static int WaterCheck(int i, int j)
        {
            Tile tile1 = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j - 1);
            Tile tileBelow2 = Framing.GetTileSafely(i, j - 2);
            Tile tileAbove = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove2 = Framing.GetTileSafely(i, j + 2);
            Tile tileLeft = Framing.GetTileSafely(i - 1, j);
            Tile tileLeft2 = Framing.GetTileSafely(i - 2, j);
            Tile tileRight = Framing.GetTileSafely(i + 1, j);
            Tile tileRight2 = Framing.GetTileSafely(i + 2, j);
            bool IsSolid(Tile tile)
            {
                return tile.active() || Main.tileSolid[tile.type];
            }
            if (tile1.active() && tileBelow.active() && tileBelow2.active() && !tileAbove.active() && !tileAbove2.active())
            {
                return 1;
            }
            if (tile1.active() && !IsSolid(tileBelow) && !IsSolid(tileBelow2) && tileAbove.active() && tileAbove2.active())
            {
                return 2;
            }
            if (tile1.active() && tileLeft.active() && tileLeft2.active() && !tileRight.active() && !tileRight2.active())
            {
                return 3;
            }
            if (tile1.active() && !tileLeft.active() && !tileLeft2.active() && tileRight.active() && tileRight2.active())
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }

        public static void MakeOvalJaggedTop(int width, int height, Vector2 startingPoint, int type, int lowRand = 10, int highRand = 20)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                    }

                    if (i == width / 2 && j == height / 2)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y + 2, WorldGen.genRand.Next(lowRand, highRand), WorldGen.genRand.Next(lowRand, highRand), type, true, 0f, 0f, true, true);
                    }
                }
            }
            int steps = 0;
            for (int i = 0; i < width; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    steps += Main.rand.Next(-1, 2);
                }

                for (int j = -6; j < height / 2 - 2 + steps; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    if (tile.type == type)
                    {
                        WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }
                }
            }
        }
        public static void MakeOvalJaggedBottom(int width, int height, Vector2 startingPoint, int type, bool overwrite = false)
        {
            int steps = 0;
            for (int i = 0; i < width; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    steps += Main.rand.Next(-1, 2);
                }

                for (int j = 0; j < height; j++)
                {
                    if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2) + steps, i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                    }
                }
            }
            int steps2 = 0;
            for (int i = 0; i < width; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    steps2 += Main.rand.Next(-1, 2);
                }

                for (int j = height / 2 - 2 + steps2; j < height + 12 + steps2; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    if (tile.type == type)
                    {
                        WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }
                }
            }
        }

        public static void TilePopulate(int[] types, Rectangle bounds)
        {
            for (int i = bounds.X; i < bounds.Width; i++)
            {
                for (int j = bounds.Y; j < bounds.Height; j++)
                {
                    int chosen = WorldGen.genRand.Next(types.Length);
                    int tile = types[chosen];

                    TileObjectData TOD = TileObjectData.GetTileData(tile, 0);
                    if (TOD.AnchorTop != AnchorData.Empty)
                    {
                        if (TileCheck2(i, j) == (int)TileSpacing.Bottom)
                        {
                            WorldGen.PlaceTile(i, j + 1, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                            for (int a = 0; a < TOD.Width; a++)
                                Framing.GetTileSafely(i + a, j).slope(0);
                        }
                    }
                    else if (TOD.AnchorBottom != AnchorData.Empty)
                    {
                        if (TileCheck2(i, j) == (int)TileSpacing.Top)
                        {
                            WorldGen.PlaceTile(i, j - TOD.Height, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                            for (int a = 0; a < TOD.Width; a++)
                                Framing.GetTileSafely(i + a, j).slope(0);
                        }
                    }
                    else if (TOD.AnchorLeft != AnchorData.Empty)
                    {
                        if (TileCheck2(i, j) == (int)TileSpacing.Right)
                        {
                            WorldGen.PlaceTile(i + 1, j, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                        }
                    }
                    else if (TOD.AnchorRight != AnchorData.Empty)
                    {
                        if (TileCheck2(i, j) == (int)TileSpacing.Left)
                        {
                            WorldGen.PlaceTile(i + TOD.Width, j, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                        }
                    }
                }
            }
        }

        public static void MakeOvalFlatTop(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (j > height / 2)
                    {
                        if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                        {
                            WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                        }
                    }
                    if (i == width / 2 && j == height / 2)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y + 2, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(10, 20), type, true, 0f, 0f, true, true);
                    }
                }
            }
        }

        public static void MakeOval(int width, int height, Vector2 startingPoint, int type, bool forced)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type, false, forced);
                    }

                    if (i == width / 2 && j == height / 2)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y + 2, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(10, 20), type, true, 0f, 0f, true, true);
                    }
                }
            }
        }
        public static void MakeCircleFromCenter(int size, Vector2 Center, int type, bool forced)
        {
            Vector2 startingPoint = new Vector2(Center.X - size * .5f, Center.Y - size * .5f);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float f = size * 0.5f;
                    if (Vector2.DistanceSquared(new Vector2(i + (int)startingPoint.X, j + (int)startingPoint.Y), startingPoint + new Vector2(size * 0.5f, size * 0.5f)) < f * f)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type, false, forced);
                    }
                }
            }
        }
        public static void DestroyCircleFromCenter(int size, Vector2 Center)
        {
            Vector2 startingPoint = new Vector2(Center.X - size * .5f, Center.Y - size * .5f);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float f = size * 0.5f;
                    if (Vector2.DistanceSquared(new Vector2(i + (int)startingPoint.X, j + (int)startingPoint.Y), startingPoint + new Vector2(size * 0.5f, size * 0.5f)) < f * f)
                    {
                        WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }
                }
            }
        }
        public static void MakeCircle(int size, Vector2 startingPoint, int type, bool forced)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float f = size * 0.5f;
                    if (Vector2.DistanceSquared(new Vector2(i + (int)startingPoint.X, j + (int)startingPoint.Y), startingPoint + new Vector2(size * 0.5f, size * 0.5f)) < f * f)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type, false, forced);
                    }
                }
            }
        }

        public static void MakeWallCircle(int size, Vector2 startingPoint, int type, bool forced)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float f = size * 0.5f;
                    if (Vector2.DistanceSquared(new Vector2(i + (int)startingPoint.X, j + (int)startingPoint.Y), startingPoint + new Vector2(size * 0.5f, size * 0.5f)) < f * f)
                    {
                        WorldGen.PlaceWall(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                    }
                }
            }
        }
        public static void RemoveStoneSlabs()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Framing.GetTileSafely(i, j);
                    if (tile.type == TileID.StoneSlab)
                    {
                        WorldGen.KillTile(i, j);
                    }
                }
            }
        }


        public static void MakeTriangle(Vector2 startingPoint, int width, int height, int slope, int type, int wallType = 0, bool pointingUp = true)
        {
            int initialStartingPosX = (int)startingPoint.X;
            int initialWidth = width;
            int initialSlope = slope;
            for (int j = 0; j < height; j++)
            {
                slope = Main.rand.Next(-1, 2) + initialSlope;
                for (int k = 0; k < slope; k++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, (int)startingPoint.Y - (j + k), type);
                    }
                }
                startingPoint.X += 1;
                width -= 2;
                j += slope - 1;
            }
        }

        public static void MakeTriangle(Vector2 startingPoint, int width, int height, int slope, int tileType = -1, int wallType = -1, bool pointingUp = true, int randFactor = 0)
        {
            int dir = 0;

            if (pointingUp) dir = 1;
            else dir = -1;

            int j = 0;

            while (j < height * dir)
            {
                for (int k = 0; k < slope + Main.rand.Next(-randFactor, randFactor + 1); k++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        if (tileType == -1)
                            WorldGen.PlaceTile(i + (int)startingPoint.X, (int)startingPoint.Y - (j + k), tileType);
                        if (wallType != -1)
                            WorldGen.PlaceWall(i + (int)startingPoint.X, (int)startingPoint.Y - (j + k), wallType);
                    }
                }
                startingPoint.X += 1;
                width -= 2;
                j += slope * dir;
            }
        }

        public static void FillRegion(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    tile.type = (ushort)type;
                    tile.active(true);
                }
            }
        }

        public static int TileCheckVertical(int positionX, int positionY, int step = 1, int maxIterations = 100)
        {
            int a = 0;
            for (int i = positionY; i < Main.maxTilesY || i > 0; i += step)
            {
                a++;
                if (WorldGen.InWorld(positionX, i, 15))
                {
                    Tile tile = Framing.GetTileSafely(positionX, i);
                    if (a == maxIterations)
                    {
                        return 0;
                    }
                    if (tile.active() && Main.tileSolid[tile.type])
                    {
                        return i;
                    }
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }

        public static int TileCheckVertical(Vector2 worldPos, int step = 1, int maxIterations = 100)
        {
            int a = 0;
            Point p = worldPos.ToTileCoordinates();

            for (int i = p.Y; i < Main.maxTilesY || i > 0; i += step)
            {
                a++;
                if (WorldGen.InWorld(p.X, i, 15))
                {
                    Tile tile = Framing.GetTileSafely(p.X, i);
                    if (a == maxIterations)
                    {
                        return 0;
                    }
                    if (tile.active() && Main.tileSolid[tile.type])
                    {
                        return i;
                    }
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }
        public static int TileCheck(int positionX, int type)
        {
            for (int i = 0; i < Main.maxTilesY; i++)
            {
                Tile tile = Framing.GetTileSafely(positionX, i);
                if (tile.type == type)
                {
                    return i;
                }
            }
            return 0;
        }

        public static void KillWall(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                     WorldGen.KillWall(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                }
            }
        }

        public static bool OvalCheck(int midX, int midY, int x, int y, int sizeX, int sizeY)
        {
            double p = Math.Pow(x - midX, 2) / Math.Pow(sizeX, 2)
                    + Math.Pow(y - midY, 2) / Math.Pow(sizeY, 2);

            return p < 1;
        }

    }
}