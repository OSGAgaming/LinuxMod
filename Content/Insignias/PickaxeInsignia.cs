using LinuxMod.Content.Projectiles.Cutscene;
using LinuxMod.Content.Tiles;
using LinuxMod.Core.Assets;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class PickaxeInsignia : InsigniaAbility
    {
        public override string InsigniaName => "Pickaxe";

        protected override void Ability()
        {
            base.Ability();
        }

    }

    public class CircleInsignia : InsigniaAbility
    {
        public override string InsigniaName => "Circle";

        protected override void Ability()
        {
            base.Ability();
        }

    }
}


