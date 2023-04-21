using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System.Collections.Generic;
using System;
using LinuxMod.Content.NPCs.Genetics;

namespace LinuxMod.Core.Mechanics
{
    public class ExampleNPCNeatAgent : NPCNeatAgent
    {
        int TimeAlive = 0;
        int TimeStuck = 0;
        float WallPenalty = 0;
        int TimeToReach = 0;
        int NewPlaces = 0;
        int StuckInArea = 0;

        List<Vector2> PlacesSeen = new List<Vector2>();

        public ExampleNPCNeatAgent(IDna Dna) : base(Dna) { }

        public ExampleNPCNeatAgent() : base() { }

        public override Agent SpawnNPC()
        {
            ExampleAgent npc = FakeNPCHost.AddNPC<ExampleAgent>();
            npc.position = Main.MouseWorld;
            npc.position.X += Main.rand.NextFloat(-8f, 8f);
            npc.position.Y += Main.rand.NextFloat(-8f, 8f);

            return npc;
        }

        public override void Refresh()
        {
            TimeAlive = 0;
            TimeStuck = 0;
            WallPenalty = 0;
            TimeToReach = 0;
            NewPlaces = 0;
            StuckInArea = 0;
            PlacesSeen.Clear();
        }

        public override void CalculateCurrentFitness()
        {
            if(TimeAlive == 0)
            {
                Fitness = 0;
                return;
            }

            float distance = Math.Max(0.01f, Vector2.Distance(Main.LocalPlayer.position, Entity.position));
            float target = 1000f;
            bool canSee = Collision.CanHitLine(Entity.Center, 1, 1, Main.LocalPlayer.Center, 1, 1);

            float DistanceFitness = canSee ? (1 + (float)Math.Pow(target / distance, 4)) : 1;
            float IllegalMoveFitness = 1 + (float)Math.Pow(1 - WallPenalty / TimeAlive, MathHelper.E);
            float ExplorationFitness = (float)Math.Pow(1 + (NewPlaces * 50) / TimeAlive, 4);
            float ReachedTargetFitness = TimeToReach != 0 ? (1 + (float)Math.Pow(400f / TimeToReach, 2f) / 50f) : 1;
            float CanSeeTargetFitness = canSee ? 1f : 0.5f;

            if(DistanceFitness > 1) Main.NewText(DistanceFitness);

            Fitness = DistanceFitness * IllegalMoveFitness * ExplorationFitness * ReachedTargetFitness * CanSeeTargetFitness;

            Fitness = Math.Max(Fitness, 0);
        }

        public override void CalculateContinuousFitness()
        {
            float distance = Math.Max(0.1f, Vector2.Distance(Main.LocalPlayer.position, Entity.position));
            TimeAlive++;

            Point tilePos = Entity.Center.ToTileCoordinates();
            int tilesAround = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    Tile t = Framing.GetTileSafely(new Point((int)MathHelper.Clamp(tilePos.X, 1, Main.maxTilesX) + i, (int)MathHelper.Clamp(tilePos.Y, 1, Main.maxTilesY) + j));
                    if (t.active() && Main.tileSolid[t.type]) tilesAround++;
                }
            }

            if (TimeAlive % 5 == 0)
            {
                bool seen = false;
                for(int i = 0; i < PlacesSeen.Count; i++)
                {
                    if (Vector2.DistanceSquared(PlacesSeen[i], Entity.Center) < 50*50)
                    {
                        seen = true;
                        break;
                    }
                }
                if (!seen)
                {
                    NewPlaces++;
                    PlacesSeen.Add(Entity.Center);
                    StuckInArea = 0;
                }
                else
                {
                    StuckInArea++;
                }
            }

            if (tilesAround >= 1)
            {
                WallPenalty++;
                if (Entity.velocity.Length() < 0.7f) TimeStuck++;
            }
            else
            {
                TimeStuck--;
                TimeStuck = Math.Max(0, TimeStuck);
            }

            if (distance <= 40f)
            {
                TimeToReach = TimeAlive;
                Kill();
            }

            if (TimeStuck > 50)
            {
                Kill();
                Fitness *= (float)Math.Pow(3,TimeAlive / 200f) * 0.5f;
            }
            
            //Fitness += (1f / distance) * 0.01f;
        }
    }
}