using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class SubworldScreenData : Mechanic
    {
        public override void AddHooks()
        {
            On.Terraria.Main.Draw += Main_Draw;
        }
        private void Main_Draw(On.Terraria.Main.orig_Draw orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);
            if (LinuxMod.Subworlds != null)
            {
                if (LinuxMod.Subworlds.IsSaving && Main.gameMenu)
                {
                    Main.numClouds = 0;
                }
                else
                {
                    if (!Main.dedServ)
                    {
                        Main.numClouds = 10;

                        Main.logo2Texture = ModContent.GetTexture("Terraria/Logo2");
                        Main.logoTexture = ModContent.GetTexture("Terraria/Logo");
                        Main.sun2Texture = ModContent.GetTexture("Terraria/Sun2");
                        Main.sun3Texture = ModContent.GetTexture("Terraria/Sun3");
                        Main.sunTexture = ModContent.GetTexture("Terraria/Sun");
                    }
                }
            }
        }
    }
}