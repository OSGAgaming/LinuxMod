using EEMod.Extensions;
using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace LinuxMod.Core
{
    public class ParticleZoneHandler : ILoad
    {
        private readonly Dictionary<string, ParticleZone> particleZones = new Dictionary<string, ParticleZone>();

        public void AddZone(string NameOfZone, int MaxNumberOfParticlesInZone)
        {
            particleZones.Add(NameOfZone, new ParticleZone(MaxNumberOfParticlesInZone));
        }
        public void AppendModule(string NameOfZone, params IParticleModule[] Module)
        {
            particleZones[NameOfZone].SetModules(Module);
        }
        public void AppendSpawnModule(string NameOfZone, IParticleSpawner Module)
        {
            particleZones[NameOfZone].SetSpawningModules(Module);
        }
        public ParticleZone Get(string NameOfZone)
        {
            return particleZones[NameOfZone];
        }
        public void Update()
        {
            foreach (ParticleZone PZ in particleZones.Values)
            {
                PZ.Update();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ParticleZone PZ in particleZones.Values)
            {
                PZ.Draw(spriteBatch);
            }
        }

        public void Load()
        {
            AddZone("Main", 40000);
        }

        public void Unload()
        {
     
        }
    }
}