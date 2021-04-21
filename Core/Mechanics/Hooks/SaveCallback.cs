using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod;
namespace LinuxMod.Core.Mechanics
{
    public class SaveCallback : Mechanic
    {
        public override void AddHooks()
        {
            On.Terraria.WorldGen.SaveAndQuitCallBack += WorldGen_SaveAndQuitCallBack;
        }
        private void WorldGen_SaveAndQuitCallBack(On.Terraria.WorldGen.orig_SaveAndQuitCallBack orig, object threadContext)
        {
            if (LinuxMod.Subworlds != null)
            {
                LinuxMod.Subworlds.IsSaving = true;

                orig(threadContext);

                LinuxMod.Subworlds.IsSaving = false;
            }
        }
    }
}