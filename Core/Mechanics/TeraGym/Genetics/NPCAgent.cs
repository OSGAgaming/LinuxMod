using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System.Collections.Generic;
using System;
using LinuxMod.Content.NPCs.Genetics;

namespace LinuxMod.Core.Mechanics
{
    public class NPCAgent : ContinuosGeneticAgent
    {
        public NPC Entity;
        public int NPCType;

        public NPCAgent(IDna Dna, int type) : base(Dna)
        {
            int id = NPC.NewNPC((int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y, type);
            Entity = Main.npc[id];

            (Entity.modNPC as Agent).network = (BaseNeuralNetwork)Dna;
        }
        public NPCAgent(int type) : base()
        {
            int id = NPC.NewNPC((int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y, type);
            Entity = Main.npc[id];
            (Entity.modNPC as Agent).network = (BaseNeuralNetwork)Dna;
        }


        public override void OnKill()
        {
            Entity.life = 0;
        }
    }
}