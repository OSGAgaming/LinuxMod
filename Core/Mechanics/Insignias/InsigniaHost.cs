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
    public class InsigniaHost
    {
        public List<InsigniaAbility> Abilities = new List<InsigniaAbility>();

        public const int ACCURACY = 50;

        public Insignia CompareInsignias(Insignia insignia)
        {
            float Lowest = 1000;
            Insignia ins = new Insignia();

            foreach(InsigniaAbility IA in Abilities)
            {
                float Accuracy = insignia.CompareInsignia(IA.Insignia);
                if(Accuracy < Lowest)
                {
                    ins = IA.Insignia;
                    Lowest = Accuracy;
                }
                Main.NewText(Accuracy);
            }

            ins.PerformanceIndicator = 1;
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
            Type[] Mechanics = LUtils.GetInheritedClasses(typeof(InsigniaAbility));
            foreach (Type type in Mechanics)
            {
                InsigniaAbility m = (InsigniaAbility)Activator.CreateInstance(type);
                m.Load();
                Abilities.Add(m);
            }
        }

        
    }
}