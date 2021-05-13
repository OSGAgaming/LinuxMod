using LinuxMod.Core.Helper.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class InsigniaMaker
    {
        public List<INode> CurrentNodeCache = new List<INode>();
        public List<INode> CurrentNode = new List<INode>();
        public List<INode> TestNodes = new List<INode>();

        public Vector2 CurrentNodeAnchor;
        public Vector2 CurrentMousePos;

        public int InsigniaLengthTrack;
        public int InsigniaLength;
        private const int ACCURACY = 50;

        public bool MouseState = Main.LocalPlayer.controlUseItem;
        float lerp;
        public void Update()
        {
            bool JustLiftedMouse = MouseState && !Main.LocalPlayer.controlUseItem;
            bool JustPressedMouse = !MouseState && Main.LocalPlayer.controlUseItem;

            lerp *= 0.98f;

            if (JustPressedMouse)
            {
                CurrentMousePos = Main.MouseScreen;
            }

            if (Main.LocalPlayer.controlUseItem)
            {
                if (InsigniaLengthTrack == 0)
                {
                    CurrentNodeCache.Clear();
                    CurrentNode.Clear();
                    CurrentNodeAnchor = Main.MouseScreen;
                }

                CurrentNodeCache.Add(new INode(Main.MouseScreen - CurrentNodeAnchor, InsigniaLengthTrack));

                for (int i = 0; i < CurrentNodeCache.Count - 1; i++)
                {
                    Vector2 p = CurrentMousePos;
                    LUtils.DrawLine(p + CurrentNodeCache[i].Position, p + CurrentNodeCache[i + 1].Position, Color.LightGoldenrodYellow);
                }

                InsigniaLengthTrack++;
            }
            else if (JustLiftedMouse && InsigniaLengthTrack > ACCURACY)
            {
                Insignia ins = new Insignia();
                ins.SetAndNormalizeNodes(CurrentNodeCache);
                LinuxMod.InsigniaSystem.CompareInsignias(ins);
            }
            else
            {
                InsigniaLengthTrack = 0;
            }



            MouseState = Main.LocalPlayer.controlUseItem;
        }

    }
}