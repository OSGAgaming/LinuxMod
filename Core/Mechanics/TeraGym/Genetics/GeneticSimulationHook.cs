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
    public class GeneticSimulationHook : Mechanic
    {
        private NPCNeatSimulation<ExampleNPCNeatAgent> simulation;
        public static GeneticSimulationHook Instance;

        public override void OnLoad()
        {
            Instance = this;
        }

        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            On.Terraria.Main.Update += Main_Update; 
        }

        private void Main_Update(On.Terraria.Main.orig_Update orig, Main self, GameTime gameTime)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!Main.gamePaused)
                {
                    simulation?.Update();
                    FakeNPCHook.Instance.FakeNPCs.Update();
                }
            }

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

            if (LinuxInput.JustClicked && simulation == null)
            {
                simulation = new NPCNeatSimulation<ExampleNPCNeatAgent>(
                    11, 3, 160,
                    (IDna) => new ExampleNPCNeatAgent(IDna), 350);
                simulation.Deploy();
            }
            if (Main.LocalPlayer.controlUp)
            {
                simulation?.Destroy();
                simulation = null;
            }

            simulation?.Draw(Main.spriteBatch);
        }
    }
}


