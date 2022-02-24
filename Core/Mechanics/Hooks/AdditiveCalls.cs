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
    public class AdditiveCalls : Mechanic
    {
        internal event Action<SpriteBatch> Additives;

        public static AdditiveCalls Instance;

        internal void AddCall(Action<SpriteBatch> IDA) => Additives += IDA;

        public override void AddHooks()
        {
            Instance = this;
            On.Terraria.Main.DrawWoF += Main_DrawWoF;
        }

        public override void Unload()
        {
            Instance = null;
            On.Terraria.Main.DrawWoF -= Main_DrawWoF;
        }
        private void Main_DrawWoF(On.Terraria.Main.orig_DrawWoF orig, Main self)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Additives?.Invoke(spriteBatch);
            Additives = null;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            orig(self);

        }
    }
}