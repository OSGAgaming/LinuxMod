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
    public class PhysicsObject : IComponent
    {
        public Vector2 Velocity;

        public Vector2 Center;
        public Vector2 OldCenter;
        public Vector2 DeltaCenter;

        public List<Module> Modules = new List<Module>();

        public PhysicsObject(params Module[] modules)
        {
            for (int i = 0; i < modules.Length; i++)
            {
                LoadModule(modules[i]);
            }
        }

        public T GetModule<T>() where T : Module
        {
            foreach (Module module in Modules)
            {
                if (module is T) return (T)module;
            }

            throw new Exception("Module Doesnt Exist");
        }

        public bool HasModule<T>() where T : Module
        {
            foreach (Module module in Modules)
            {
                if (module is T) return true;
            }

            return false;
        }


        public void LoadModule(Module Module)
        {
            var conditional = Module.GetType().GetCustomAttributes(true);
            int hasModule = 0;

            foreach (Module mod in Modules)
            {
                if (typeof(Module) == mod.GetType())
                    throw new InvalidDataException("Module already Exists");

                if(conditional.Length > 0)
                if ((conditional[0] as NeedsAttribute).type == mod.GetType()) hasModule++;
            }

            if(hasModule == 0 && conditional.Length > 0) throw new InvalidDataException("This object does not have the required modules");

            Module.Object = this;

            Module.Load();

            Mechanic.GetMechanic<PhysicsObjectHook>()?.Objects.AddObject(this);
            Modules.Add(Module);
        }


        public void Draw(SpriteBatch spritebatch)
        {
            foreach (Module module in Modules)
            {
                module.Draw(spritebatch);
            }
        }

        public void Update()
        {
           

            OldCenter = Center;
            foreach (Module module in Modules)
            {
                module.Update();
            }
            Center += Velocity;
        }
    }
}