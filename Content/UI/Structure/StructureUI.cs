
using LinuxMod.Core.Assets;
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace LinuxMod.Core
{
    public enum StructureState
    {
        None,
        Create,
        Copy
    }
    public class StructureUI : UIState
    {
        StructureState State;

        private Vector2 Position;
        private Vector2 Width;
        private bool isSaving;
        internal static string StructurePath => $@"{Main.SavePath}\Mod Sources\LinuxMod\Content\Structures";
        internal static string StructureName { get; set; } = "Test";
        private Stream FileWriteStream => File.OpenWrite(StructurePath + $@"\{StructureName}.linstr");
        private Stream FileReadStream => File.OpenRead(StructurePath + $@"\{StructureName}.linstr");

        DimensionUIElement Panel = new DimensionUIElement(Main.magicPixel);
        public override void OnInitialize()
        {
            Panel.VAlign = 0.4f;
            Panel.SetW(200);
            Panel.SetH(40);
            Append(Panel);

            UIImageButton Create = new UIImageButton(Asset.GetTexture("GUI/Delete"));

            Create.OnClick += SetCreateState;
            Create.HAlign = 0.2f;
            Create.VAlign = 0f;
            Panel.Append(Create);

            UIImageButton UnCreate = new UIImageButton(Asset.GetTexture("GUI/Delete"));

            UnCreate.OnClick += UnsetCreateState;
            UnCreate.HAlign = 0.5f;
            UnCreate.VAlign = 0f;
            Panel.Append(UnCreate);

            UIImageButton Save = new UIImageButton(Asset.GetTexture("GUI/Delete"));

            Save.OnClick += SaveArea;
            Save.HAlign = 0.8f;
            Save.VAlign = 0f;
            Panel.Append(Save);
        }

        public override void OnActivate()
        {
            Panel.HAlign = -0.5f;
        }


        private void SetCreateState(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.NewText("Drag area to copy");
            State = StructureState.Create;
        }

        private void UnsetCreateState(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.NewText("Structure Debug Off");
            State = StructureState.None;
        }
        public void SaveStructure()
        {
            Main.NewText("Saved to " + StructurePath + "!");
            StructureWriter writer = new StructureWriter(FileWriteStream);
            writer.Write(Position.ToTileCoordinates(), Width.ToTileCoordinates());
        }
        private void SaveArea(UIMouseEvent evt, UIElement listeningElement) => SaveStructure();
        private bool Vb = true;
        private bool Cb = true;
        public override void Update(GameTime gameTime)
        {
            Panel.HAlign *= 0.94f;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.V))
                {
                    if (Vb)
                    {
                        try
                        {
                            StructureReader reader = new StructureReader(FileReadStream);
                            reader.Read(Main.MouseWorld.ToTileCoordinates());
                        }
                        catch
                        {
                            Main.NewText("Failed to Deserialize/Place");
                        }
                    }

                    Vb = false;
                }
                else
                {
                    Vb = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    if(Cb)
                    {
                        SaveStructure();
                        Cb = false;
                        isSaving = true;
                    }
                }
                else
                {
                    Cb = true;
                }
            }
        }

        public override void OnDeactivate()
        {
            isSaving = false;
            base.OnDeactivate();
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 width = Main.MouseWorld - Position;
            Rectangle CopyArea = LinuxTechTips.Rect(Position.ForDraw().RoundTo(16), Width.RoundTo(16));
            if (State == StructureState.Create)
            {
                if (Main.LocalPlayer.controlUseItem)
                {
                    if (LinuxInput.JustClicked)
                    {
                        Position = Main.MouseWorld;
                        isSaving = false;
                    }
                    Width = width;
                    LinuxTechTips.DrawRectangle(CopyArea, Color.Red, 2f);
                }
                else
                {
                    LinuxTechTips.DrawRectangle(CopyArea, Color.Red * (float)Math.Sin(Main.GameUpdateCount / 5f), 2f);
                }
            }

            if(isSaving)
            {
                Vector2 Pos = Main.MouseWorld.RoundTo(16).ForDraw();
                int w = (int)Width.X / 16;
                int h = (int)Width.Y / 16;
                for (int i = 0; i < w; i++)
                {
                    for(int j = 0; j < h; j++)
                    {
                        Point p = Position.ToTileCoordinates();
                        Tile tile = Framing.GetTileSafely(p.X + i, p.Y + j);
                        if (tile.wall > 0)
                        {
                            spriteBatch.Draw(Main.wallTexture[tile.wall], Pos + new Vector2(i * 16, j * 16), new Rectangle(tile.wallFrameX(), tile.wallFrameY(), 36, 36), Color.White * (float)Math.Sin(Main.GameUpdateCount / 5f) * 0.5f);
                        }
                        if (tile.active())
                        {
                            spriteBatch.Draw(Main.tileTexture[tile.type], Pos + new Vector2(i * 16, j *  16), new Rectangle(tile.frameX, tile.frameY, 16, 16) ,Color.White * (float)Math.Sin(Main.GameUpdateCount / 5f) * 0.5f);
                        }
                    }
                }
            }
        }
    }
}