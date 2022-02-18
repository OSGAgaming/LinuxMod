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
        public InsigniaMaker Host = new InsigniaMaker();
        public override void AddHooks()
        {
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
            On.Terraria.Main.DrawPlayer += Main_DrawPlayer;
        }


        private void Main_DrawPlayer(On.Terraria.Main.orig_DrawPlayer orig, Main self, Player drawPlayer, Vector2 Position, float rotation, Vector2 rotationOrigin, float shadow)
        {
            if(!drawPlayer.GetModPlayer<InsigniaPlayer>().Invisible)
              orig(self, drawPlayer, Position, rotation, rotationOrigin, shadow);
        }

        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            Host.Update();
            LinuxMod.GetLoadable<InsigniaHost>().Draw(Main.spriteBatch);

            List<InsigniaAbility> Abilities = LinuxMod.GetLoadable<InsigniaHost>().Abilities;

            for(int i = 0; i<Abilities.Count; i++)
            {
                //Abilities[i].Insignia.DebugDraw(Main.LocalPlayer.Center + new Vector2(120 * i,0), 100);
            }
            orig(self);

        }
    }
}