
using LinuxMod.Content.Projectiles.Cutscene;
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace LinuxMod.Content.Tiles
{
    public class WallWellTile : ModTile
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "LinuxMod/Assets/Tiles/Cutscene/WallWellTile";
            return base.Autoload(ref name, ref texture);
        }
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.addTile(Type);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, TileObjectData.newTile.Width, 0);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Wall Well");
            AddMapEntry(new Color(255, 168, 28), name);
            disableSmartCursor = true;
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (Framing.GetTileSafely(i, j).frameX == 0 && Framing.GetTileSafely(i, j).frameY == 0)
            {
                if (!WaterPositionCache.Positions.Contains(new Vector2(i * 16, j * 16)))
                {
                    Projectile.NewProjectile(new Vector2(i * 16, j * 16), Vector2.Zero, ModContent.ProjectileType<WallWellWater>(), 0, 0);
                    WaterPositionCache.Positions.Add(new Vector2(i * 16, j * 16));
                }
            }
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (WaterPositionCache.Positions.Contains(new Vector2(i * 16, j * 16)))
            {
                WaterPositionCache.Positions.Remove(new Vector2(i * 16, j * 16));
            }
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Framing.GetTileSafely(i, j).frameX == 0 && Framing.GetTileSafely(i, j).frameY == 0)
            {
                if (!WaterPositionCache.Positions.Contains(new Vector2(i * 16, j * 16)))
                {
                    WaterPositionCache.Positions.Add(new Vector2(i * 16, j * 16));
                    Projectile.NewProjectile(new Vector2(i * 16, j * 16), Vector2.Zero, ModContent.ProjectileType<WallWellWater>(), 0, 0);
                }
            }
        }
    }
}