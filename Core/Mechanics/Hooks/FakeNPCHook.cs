using LinuxMod.Content.NPCs.Genetics;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class FakeNPCHook : Mechanic
    {
        public FakeNPCHost FakeNPCs;
        public static FakeNPCHook Instance;

        public override void OnLoad()
        {
            FakeNPCs = new FakeNPCHost();
            Instance = this;
        }

        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            On.Terraria.Main.Update += Main_Update;
        }

        private void Main_Update(On.Terraria.Main.orig_Update orig, Main self, GameTime gameTime)
        {
            //FakeNPCs.Update();
            orig(self, gameTime);
        }

        public override void Unload()
        {
            On.Terraria.Main.DrawWoF -= Main_DrawWoF;
            Instance = null;
        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            orig(self);

            FakeNPCs.Draw(Main.spriteBatch);
        }
    }
}


