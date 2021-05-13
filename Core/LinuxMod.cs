using LinuxMod.Core.Mechanics;
using LinuxMod.Core.Mechanics.Verlet;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace LinuxMod.Core
{
	public partial class LinuxMod : Mod
	{
        public static SubworldInstanceManager Subworlds;
        public static ParticleZoneHandler GlobalZone;
        public static VerletSystem verletSystem;
        public static UIManager UI;
        public static InsigniaHost InsigniaSystem;
        private GameTime lastGameTime;
        public override void Load()
        {
            InsigniaSystem = new InsigniaHost();
            Subworlds = new SubworldInstanceManager();
            GlobalZone = new ParticleZoneHandler();
            verletSystem = new VerletSystem();
            UI = new UIManager();
            LoadHotkeys();
            GlobalZone.AddZone("Main", 40000);
            ShaderLoading();
            UI.LoadUI();
            AutoloadMechanics.Load();
            InsigniaSystem.Load();
        }
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            ScreenMapPass.Instance.Maps.OrderedShaderPass();
            ScreenMapPass.Instance?.Maps?.OrderedRenderPassBatched(Main.spriteBatch, Main.graphics.GraphicsDevice);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                LegacyGameInterfaceLayer LinuxUILayer = new LegacyGameInterfaceLayer("LinuxMod: LinuxInterface", delegate
                {
                    if (lastGameTime != null)
                    {
                        UI.Draw(lastGameTime);
                    }

                    return true;
                }, InterfaceScaleType.Game);
                layers.Insert(mouseTextIndex, LinuxUILayer);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            UI.Update(gameTime);
            UIControls();
            lastGameTime = gameTime;
        }
        public void UIControls()
        {
            if (InsigniaActivator.JustPressed)
            {
                UI.ToggleState<InsigniaUI>();
            }
        }
        public override void Unload()
        {
            Subworlds = null;
            GlobalZone = null;
            UI.UnLoad();
        }
    }
}