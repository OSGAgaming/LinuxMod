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
    public class CubeInsignia : InsigniaAbility
    {
        public override string InsigniaName => "Cube";

        protected override void OnActive(Player player)
        {
            player.Hurt(null, 10, 1);
        }

    }
}


