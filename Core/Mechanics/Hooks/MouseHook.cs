using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod;
namespace LinuxMod.Core.Mechanics
{
    public class LinuxInput : Mechanic
    {
        public static Vector2 DeltaMouse;
        public static bool JustClicked;
        public static bool JustReleased;

        private Vector2 OldMouse;
        private bool OldMouseState;
        public override void AddHooks()
        {
            Main.OnPreDraw += Main_OnPreDraw;
        }

        public override void Unload()
        {
            Main.OnPreDraw -= Main_OnPreDraw;
        }
        private void Main_OnPreDraw(GameTime obj)
        {
            DeltaMouse = OldMouse - Main.MouseWorld;
            JustClicked = Main.LocalPlayer.controlUseItem && !OldMouseState;
            JustReleased = !Main.LocalPlayer.controlUseItem && OldMouseState;

            OldMouse = Main.MouseWorld;
            OldMouseState = Main.LocalPlayer.controlUseItem;
        }

    }
}