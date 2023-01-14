using LinuxMod.Core.Mechanics;
using LinuxMod.Core.Mechanics.Interfaces;
using LinuxMod.Core.Mechanics.Verlet;
using LinuxMod.Core.Subworlds;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace LinuxMod.Core
{
    public partial class LinuxMod : Mod
    {
        private GameTime lastGameTime;

        public static List<ILoad> Loadables;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Loadables = new List<ILoad>();

                Type[] loadables = LinuxTechTips.GetInheritedClasses(typeof(ILoad));
                foreach (Type type in loadables)
                {
                    ILoad loadable = Activator.CreateInstance(type) as ILoad;
                    loadable.Load();

                    Loadables.Add(loadable);
                }

                LoadHotkeys();
                ShaderLoading();

                ModelLoader.Load();
                AutoloadMechanics.Load();
                ModuleHostLoader.Load();
                LocalRenderer.Load();
                ParticulateStep.Load();

                Debug.WriteLine("Loaded!");
            }
        }

        public static T GetLoadable<T>()
        {
            foreach (ILoad loadable in Loadables)
                if (loadable is T) return (T)loadable;

            throw new NullReferenceException("Loadable could not be found");
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
                        GetLoadable<UIManager>().Draw(lastGameTime);
                    }

                    return true;
                }, InterfaceScaleType.Game);
                layers.Insert(mouseTextIndex, LinuxUILayer);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            GetLoadable<UIManager>().Update(gameTime);
            UIControls();
            lastGameTime = gameTime;
        }
        public void UIControls()
        {
            if (InsigniaActivator.JustPressed)
            {
                GetLoadable<UIManager>().ToggleState<InsigniaUI>();
                GetLoadable<UIManager>().ToggleState<StructureUI>();
            }
        }
        public override void Unload()
        {
            ModelLoader.Unload();
            LocalRenderer.Unload();
            AutoloadMechanics.Unload();
            ModuleHostLoader.Unload();
            ParticulateStep.Unload();

            UnloadShaders();

            for (int i = 0; i < Loadables.Count; i++)
                Loadables[i] = null;

            Loadables.Clear();

            Loadables = null;

            Debug.WriteLine("Unloaded!");
            Debug.WriteLine("");
        }
    }
}