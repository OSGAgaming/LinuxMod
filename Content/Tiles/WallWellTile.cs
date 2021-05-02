
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace LinuxMod.Content.Tiles
{

    public class WellTE : ModTileEntity
    {
        public bool hasOrb = false;

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            return tile.active();
        }
        public override void PostGlobalUpdate()
        {
            if (Framing.GetTileSafely(i, j).frameX == 0)
            {
                ScreenMapPass.Instance.GetMap("Sewer").DrawToTarget((SpriteBatch sb) =>
                {
                    Vector2 v = new Vector2(i * 16, j * 16).ForDraw();
                    Point p1 = v.ToPoint();
                    Point p2 = new Point(100, 100);
                    sb.Draw(Main.magicPixel, new Rectangle(p1.X, p1.Y, p2.X, p2.Y), Color.AliceBlue);
                });


                ScreenMapPass.Instance.GetMap("Sewer").ApplyShader();
            }
        }
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }

            return Place(i, j);
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(hasOrb);
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            hasOrb = reader.ReadBoolean();
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                [nameof(hasOrb)] = hasOrb
            };
        }

        public override void Load(TagCompound tag)
        {
            hasOrb = tag.GetBool(nameof(hasOrb));
        }
    }

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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<WellTE>().Hook_AfterPlacement, -1, 0, true);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Wall Well");
            AddMapEntry(new Color(255, 168, 28), name);
            disableSmartCursor = true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {

        }
    }
}