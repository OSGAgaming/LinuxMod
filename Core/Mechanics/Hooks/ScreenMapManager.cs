
using LinuxMod.Core.Mechanics.ScreenMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;

namespace LinuxMod.Core.Mechanics
{
    public class ScreenMapPass : Mechanic
    {
        public Map Maps;

        public static ScreenMapPass Instance;
        public MapPass GetMap(string String) => Maps.Get(String);
        public override void OnLoad()
        {
            Maps = new Map();

            Maps.AddMap("CutsceneWaterReflection", 0, new CutsceneWater());
            Maps.AddMap("Sewer", 1, new SewerWater());
            Instance = this;
        }
        public override void AddHooks()
        {
            Main.OnPostDraw += Main_OnPostDraw;
        }

        private void Main_OnPostDraw(GameTime obj)
        {
        }
    }


}