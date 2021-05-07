using LinuxMod.Core.Mechanics;
using LinuxMod.Core.Mechanics.Verlet;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace LinuxMod.Core
{
	public partial class LinuxMod : Mod
	{
        public static SubworldInstanceManager Subworlds;
        public static ParticleZoneHandler GlobalZone;
        public static VerletSystem verletSystem;
        public override void Load()
        {
            Subworlds = new SubworldInstanceManager();
            GlobalZone = new ParticleZoneHandler();
            verletSystem = new VerletSystem();

            GlobalZone.AddZone("Main", 40000);
            ShaderLoading();
            AutoloadMechanics.Load();
        }
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            ScreenMapPass.Instance.Maps.OrderedShaderPass();
            ScreenMapPass.Instance?.Maps?.OrderedRenderPassBatched(Main.spriteBatch, Main.graphics.GraphicsDevice);
        }
        public override void Unload()
        {
            Subworlds = null;
            GlobalZone = null;
        }
    }
}