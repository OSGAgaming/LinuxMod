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
    [Serializable]
    public struct INode
    {
        public Vector2 Position;
        public float Progression;

        public INode(Vector2 Position, float Progression)
        {
            this.Position = Position;
            this.Progression = Progression;
        }
    }
    public abstract class InsigniaAbility
    {
        internal static string InsigniaPath => $@"{Main.SavePath}\Mod Sources\LinuxMod\Content\Insignias";

        private Stream FileStream => File.OpenRead(InsigniaPath + $@"\{InsigniaName}.insg");

        public virtual string InsigniaName { get; set; }

        internal Insignia Insignia { get; set; }

        internal virtual int AbilityLength => 600;

        internal int AbilityTimer { get; set; }

        protected InsigniaMaker Maker => Mechanic.GetMechanic<InsigniaHook>().Host;

        internal void Load()
        {
            Insignia = Insignia.Deserialize(FileStream);
        }

        private int prevTimer;
        public void Update()
        {
            if (AbilityTimer > 0)
            {
                OnActive(Main.LocalPlayer);
                AbilityTimer--;
            }
            else UpdatePassive(Main.LocalPlayer);

            if (prevTimer == 1 && AbilityTimer == 0) 
            {
                Deactivate();
            }

            prevTimer = AbilityTimer;
        }

        public void Draw(SpriteBatch sb)
        {
            if (AbilityTimer > 0)
            {
                DrawEffects(Main.LocalPlayer, sb);
            }

            DrawAlways(Main.LocalPlayer, sb);
        }

        public void Activate()
        {
            if (AbilityTimer <= 0)
            {
                AbilityTimer = AbilityLength;
                OnActivate(Main.LocalPlayer);
            }
        }

        public void Deactivate()
        {
            AbilityTimer = 0;
            OnDeactivate(Main.LocalPlayer);
        }
        protected virtual void OnActive(Player player) { }

        protected virtual void DrawEffects(Player player, SpriteBatch sb) { }

        protected virtual void DrawAlways(Player player, SpriteBatch sb) { }

        protected virtual void UpdatePassive(Player player) { }

        protected virtual void OnActivate(Player player) { }

        protected virtual void OnDeactivate(Player player) { }
    }

   
}