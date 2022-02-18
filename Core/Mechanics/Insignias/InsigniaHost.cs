using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class InsigniaHost : ILoadable
    {
        public List<InsigniaAbility> Abilities = new List<InsigniaAbility>();

        public const int ACCURACY = 10;
        public static string DebugTry = "bab";

        public void Update()
        {
            foreach(InsigniaAbility IA in Abilities)
            {
                IA.Update();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (InsigniaAbility IA in Abilities)
            {
                IA.Draw(sb);
            }
        }
        public InsigniaAbility CompareInsignias(Insignia insignia)
        {
            float Lowest = 1000;
            InsigniaAbility ins = null;
            string Name = "";
            foreach(InsigniaAbility IA in Abilities)
            {
                float Accuracy = insignia.CompareInsignia(IA.Insignia);
                Main.NewText(IA.InsigniaName + ": " + ((1 - Accuracy) * 100) + "%");
                if(Accuracy < Lowest)
                {
                    ins = IA;
                    Name = IA.InsigniaName;
                    Lowest = Accuracy;
                }
            }

            if (ins != null)
            {
                DebugTry = Name;
                ins.Activate();
                ins.Insignia.PerformanceIndicator = 1;
            }
            return ins;
        }

        public static Insignia CreateInsignia(List<INode> nodes, bool Normalized = false)
        {
            Insignia ins = new Insignia();
            if (!Normalized) ins.SetNodes(nodes);
            else ins.SetAndNormalizeNodes(nodes);

            return ins;
        }

        public void Load()
        {
            Type[] Mechanics = LinuxTechTips.GetInheritedClasses(typeof(InsigniaAbility));
            foreach (Type type in Mechanics)
            {
                InsigniaAbility m = (InsigniaAbility)Activator.CreateInstance(type);
                m.Load();
                Abilities.Add(m);
            }
        }

        public void Unload()
        {
            Abilities.Clear();
        }
    }
}