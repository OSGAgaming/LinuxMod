using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System;
using System.Collections.Generic;

namespace LinuxMod.Core.Mechanics
{
    public class FakeNPCHost
    {
        private List<FakeNPC> fakeNPCs = new List<FakeNPC>();
        public static FakeNPCHost Instance;

        public FakeNPCHost()
        {
            Instance = this;
        }

        public static T AddNPC<T>() where T : FakeNPC, new() => Instance.Add<T>();
        public static void DespawnNPC(int i) => Instance.Despawn(i);
        public static void DespawnNPC(FakeNPC n) => Instance.Despawn(n);

        public T Add<T>() where T : FakeNPC, new()
        {
            for (int i = 0; i < fakeNPCs.Count; i++)
            {
                FakeNPC n = fakeNPCs[i];
                if (!n.active || n == null)
                {
                    T replaceNPC = new T();
                    replaceNPC.whoAmI = i;
                    fakeNPCs[i] = replaceNPC;
                    return replaceNPC;
                }
            }

            T newNPC = new T();
            newNPC.whoAmI = fakeNPCs.Count;

            fakeNPCs.Add(newNPC);

            return newNPC;
        }

        public void Despawn(int i) => fakeNPCs[i].active = false;
        public void Despawn(FakeNPC n) => fakeNPCs[n.whoAmI].active = false;

        public void Update()
        {
            foreach(FakeNPC n in fakeNPCs)
            {
                if(n.active && n != null) n.Update();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (FakeNPC n in fakeNPCs)
            {
                if (n.active && n != null) n.Draw(sb);
            }
        }
    }
}