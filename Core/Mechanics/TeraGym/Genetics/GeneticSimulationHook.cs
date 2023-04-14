using LinuxMod.Content.NPCs.Genetics;
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
    public class GeneticSimulationHook : Mechanic
    {
        private NPCSimulation<ExampleNPCAgent> simulation;

        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            Main.OnPreDraw += Main_OnPreDraw;
        }

        public override void Unload()
        {
            On.Terraria.Main.DrawWoF -= Main_DrawWoF;
            Main.OnPreDraw -= Main_OnPreDraw;
        }
        private void Main_OnPreDraw(GameTime obj)
        {
            if (!Main.gamePaused) simulation?.Update();
        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            orig(self);

            if (LinuxInput.JustClicked)
            {
                simulation = new NPCSimulation<ExampleNPCAgent>(
                    ModContent.NPCType<ExampleAgent>(), 50, 
                    (IDna, type) => new ExampleNPCAgent(IDna, type),
                    (type) => new ExampleNPCAgent(type),
                    0.03f, 120);

                simulation.Deploy();
            }
            if (Main.LocalPlayer.controlUp)
            {
                simulation = null;
            }
        }
    }
}


