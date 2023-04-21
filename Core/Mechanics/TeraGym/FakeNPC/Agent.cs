using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System;
using System.Collections.Generic;

namespace LinuxMod.Core.Mechanics
{
    public class Agent : FakeNPC
    {
        public IDna network;

        public virtual List<float> FeedInputs() { return null; }
        public virtual void Response(float[] output) { }

        public override void AI()
        {
            if (network == null) return;

            network.Compute(FeedInputs().ToArray());

            Response(network.Response);
        }
    }
}