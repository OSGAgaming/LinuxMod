using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class RightArm : Arm
    {
        public override string ID => "R_Arm";

        public override string OtherArm => "L_Arm";

        public override int StaticSide => 1;
    }
}