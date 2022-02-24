
using LinuxMod.Core.Mechanics.ScreenMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core.Mechanics
{
    public class ScreenMapPass : Mechanic
    {
        public Map Maps;

        public static ScreenMapPass Instance;
        public MapPass GetMap(string String) => Maps?.Get(String);

        public override void Unload()
        {
            Maps = null;
            Instance = null;
        }

        public override void OnLoad()
        {
            Maps = new Map();

            if (!Main.dedServ)
            {
                //autobind
                Mod mod = ModContent.GetInstance<LinuxMod>();

                foreach (Type t in mod.Code.GetTypes())
                {
                    if (t.IsSubclassOf(typeof(MapPass)))
                    {
                        var state = (MapPass)Activator.CreateInstance(t);
                        Maps.AddMap(t.Name, state);
                    }
                }

                Instance = this;
            }
        }
    }
}