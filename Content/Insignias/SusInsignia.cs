using LinuxMod.Content.Projectiles.Cutscene;
using LinuxMod.Content.Tiles;
using LinuxMod.Core.Assets;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class SusInsignia : InsigniaAbility
    {
        public override string InsigniaName => "Sus";

        float Acceleration = 0.2f;
        Vector2 Velocity;
        bool IsZooming = false;
        protected override void OnActive(Player player)
        {
            IsZooming = true;

            player.GetModPlayer<LinuxPlayer>().ReciprocateZoom(2, 66f);
            Acceleration = 0.2f;
            player.GetModPlayer<InsigniaPlayer>().Invisible = true;

            Vector2 size = new Vector2(15, 15);
            KeyboardState state = Keyboard.GetState();

            player.width = (int)player.DefaultSize.X;
            Velocity *= 0.95f;
            player.position.Y += player.height - size.Y;

            player.height = (int)size.Y;
            player.width = (int)size.X;
        }

        protected override void UpdatePassive(Player player)
        {
            if (Math.Abs(1 - player.GetModPlayer<LinuxPlayer>().Zoom) < 0.01f)
                IsZooming = false;

            if (IsZooming)
                player.GetModPlayer<LinuxPlayer>().ReciprocateZoom(1, 66f);
        }
        protected override void OnActivate(Player player)
        {
            player.GetModPlayer<LinuxPlayer>().ScreenShake = 20;
        }
        protected override void OnDeactivate(Player player)
        {
            Velocity = Vector2.Zero;
            Vector2 size = new Vector2(15, 15);
            player.position.Y -= player.DefaultSize.Y - size.Y;
            player.GetModPlayer<InsigniaPlayer>().Invisible = false;
            player.height = (int)player.DefaultSize.Y;
            player.width = (int)player.DefaultSize.X;
            player.immuneAlpha = 0;
        }
    }
}


