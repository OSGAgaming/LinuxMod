
using LinuxMod.Core.Assets;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace LinuxMod.Core
{
    public class InsigniaUI : UIState
    {
        public InsigniaMakerPanel panel = new InsigniaMakerPanel();
        NewUITextBox Acc = new NewUITextBox("Count");
        NewUITextBox Name = new NewUITextBox("Name");
        public override void OnInitialize()
        {

            panel.Height.Set(500, 0);
            panel.Width.Set(500, 0);
            panel.HAlign = 0.5f;
            panel.VAlign = 0.01f;
            Append(panel);

            UIImageButton clear = new UIImageButton(Asset.GetTexture("Tiles/Cutscene/WallWellTile"));

            clear.OnClick += Clear_OnClick;
            clear.HAlign = 0.9f;
            clear.VAlign = 0.9f;
            panel.Append(clear);

            UIImageButton create = new UIImageButton(Asset.GetTexture("Tiles/Cutscene/WallWellTile"));

            create.OnClick += CreateInsignia;
            create.HAlign = 0.1f;
            create.VAlign = 0.1f;
            panel.Append(create);

            Acc.HAlign = 0.5f;
            Acc.VAlign = 0.9f;
            Acc.Height.Set(40, 0);
            Acc.Width.Set(50, 0);

            panel.Append(Acc);

            Name.HAlign = 0.1f;
            Name.VAlign = 0.9f;
            Name.Height.Set(40, 0);
            Name.Width.Set(50, 0);
            panel.Append(Name);
        }



        private void Clear_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            panel.CanClick = false;
            panel.Clear();
        }

        private void CreateInsignia(UIMouseEvent evt, UIElement listeningElement)
        {
            panel.CanClick = false;

            Main.NewText("Insignia Created!");

            string Path = InsigniaAbility.InsigniaPath + $@"\{Name.currentString}.insg";
            InsigniaHost.CreateInsignia(panel.PNodes, true).Serialize(File.OpenWrite(Path));

            Main.NewText("Saved at " + Path);
        }

        public override void Update(GameTime gameTime)
        {
        }
    }

    public class InsigniaMakerPanel : UIElement
    {
        public List<INode> PNodes = new List<INode>();
        public int NodeIndex;
        public bool CanClick;

        public override void OnInitialize()
        {
            //PNodes.Add(new INode(new Vector2(0.5f, 0.2f), 1));
        }

        internal void Clear()
        {
            PNodes.Clear();
            NodeIndex = 0;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Main.magicPixel, GetDimensions().ToRectangle(), Color.White);
            for (int i = 0; i < PNodes.Count; i++)
            {
                int size = 10;

                Vector2 point = GetDimensions().Position() + PNodes[i].Position;
                LUtils.UITextToCenter(PNodes[i].Progression.ToString(), Color.Red, point - new Vector2(0, 20), 1);
                spriteBatch.Draw(Main.magicPixel, new Rectangle((int)point.X - size / 2, (int)point.Y - size / 2, size, size), Color.Red);

                if (i > 0)
                {
                    Vector2 point2 = GetDimensions().Position() + PNodes[i - 1].Position;

                    LUtils.DrawLine(point, point2, Color.Red);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Click(UIMouseEvent evt)
        {
            if (CanClick && !Main.blockInput)
            {
                Vector2 rPos = new Vector2(Main.mouseX, Main.mouseY) - GetDimensions().Position();

                PNodes.Add(new INode(rPos, NodeIndex));

                NodeIndex++;
            }

            CanClick = true;
            base.Click(evt);
        }
    }
}