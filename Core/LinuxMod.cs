using LinuxMod.Core.Mechanics;
using Terraria.ModLoader;

namespace LinuxMod.Core
{
	public partial class LinuxMod : Mod
	{
        public static SubworldInstanceManager Subworlds;
        public static ParticleZoneHandler GlobalZone;
        public override void Load()
        {
            Subworlds = new SubworldInstanceManager();
            GlobalZone = new ParticleZoneHandler();
            GlobalZone.AddZone("Main", 40000);
            ShaderLoading();
            AutoloadMechanics.Load();
        }

        public override void Unload()
        {
            Subworlds = null;
            GlobalZone = null;
        }
    }
}