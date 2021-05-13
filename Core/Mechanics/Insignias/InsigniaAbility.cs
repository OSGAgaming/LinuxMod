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

        internal void Load()
        {
            Insignia = Insignia.Deserialize(FileStream);
        }

        protected virtual void Ability() { }
    }

   
}