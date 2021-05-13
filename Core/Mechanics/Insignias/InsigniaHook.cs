using LinuxMod.Core.Helper.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class InsigniaHook : Mechanic
    {
        InsigniaMaker Host = new InsigniaMaker();
        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            Host.Update();

            List<InsigniaAbility> Abilities = LinuxMod.InsigniaSystem.Abilities;

            for(int i = 0; i<Abilities.Count; i++)
            {
                Abilities[i].Insignia.DebugDraw(Main.LocalPlayer.Center + new Vector2(100 * i,0), 100);
            }
            orig(self);

        }
    }
}