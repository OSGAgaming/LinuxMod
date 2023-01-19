
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Mechanics;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.ModLoader;

namespace LinuxMod.Core
{
    public class ParticulateStep
    {
        public static Dictionary<int, ParticulateStep> actions = new Dictionary<int, ParticulateStep>();

        public static void Load()
        {
            LoadStepToID<ImmovableSolid>(1);
            LoadStepToID<SandStep>(2);
            LoadStepToID<WaterStep>(3);
            LoadStepToID<CoinStep>(4);
            LoadStepToID<GasStep>(5);
        }

        public static void Unload() => actions.Clear();

        public static void LoadStepToID<T>(int ID) where T : ParticulateStep, new() => actions.Add(ID, new T());

        public virtual void Step(Particulate p, ParticulateField particles) { }

        public virtual void Draw(SpriteBatch sb, Particulate p, ParticulateField particles) { }

        public virtual void InitialStep(Particulate p, ParticulateField particles) { }
    }
}