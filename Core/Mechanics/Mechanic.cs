using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class Mechanic
    {
        public LinuxMod ModInstance => ModContent.GetInstance<LinuxMod>();
        public float ElapsedTicks => Main.GameUpdateCount;
        public virtual void OnDraw(SpriteBatch spriteBatch) { }
        public virtual void OnUpdate() { }
        public virtual void OnLoad() { }
        public virtual void AddHooks() { }

        public void Draw(SpriteBatch spritebatch)
        {
            OnDraw(spritebatch);
        }

        public void Update()
        {
            OnUpdate();
        }
        public void Load()
        {
            AddHooks();
            OnLoad();
        }
    }
}