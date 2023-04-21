using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using LinuxMod.Core.Helper;
using Terraria.ModLoader;
using LinuxMod.Core;
using LinuxMod.Core.Assets;
using Terraria.ID;
using LinuxMod.Core.Mechanics.Verlet;

namespace LinuxMod.Content.NPCs.Genetics
{
    public class AgentNPC : ModNPC
    {
        public IDna network;

        public virtual List<float> FeedInputs() { return null; }
        public virtual void Response(float[] output) { }

        public override void AI()
        {
            if(network == null) return;

            network.Compute(FeedInputs().ToArray());

            Response(network.Response);
        }
    }
}
