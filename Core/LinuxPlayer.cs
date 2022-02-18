
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;
using LinuxMod.Core.Subworlds;
using LinuxMod;
using LinuxMod.Core.Subworlds.LinuxSubworlds;

namespace LinuxMod.Core
{
    public class LinuxPlayer : ModPlayer
    {
        public int ScreenShake;
        public float Zoom;
        public Vector2 ScreenPositionTarget;
        public Vector2 Offset;
        public void ReciprocateZoom(float target, float per) => Zoom += (target - Zoom) / per;

        public void SetScreenPosition(Vector2 target, float ease = 64f) => Offset += (target - player.Center - Offset) / ease;

        public override void ModifyScreenPosition()
        {
            if (ScreenShake > 0) ScreenShake--;

            Main.screenPosition += new Vector2(Main.rand.NextFloat(ScreenShake), Main.rand.NextFloat(ScreenShake));
            Main.screenPosition += Offset;
            //Main.GameZoomTarget = Zoom;

            Offset -= Offset / 16f;
        }
        public override void OnEnterWorld(Player player)
        {
            if (!Main.LocalPlayer.GetModPlayer<SubworldPlayer>().InSubworld)
                SubworldManager.EnterSubworld<SeamapSubworld>();
        }
    }
}
