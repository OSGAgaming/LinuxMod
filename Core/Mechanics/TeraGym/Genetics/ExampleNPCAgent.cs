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
    public class ExampleNPCAgent : NPCAgent
    {
        int TimeAlive = 0;
        int TimeStuck = 0;
        int TimeToReach = 0;
        int TimeInWall = 0;
        int NewPlaces = 0;

        List<Vector2> PlacesSeen = new List<Vector2>();
        Vector2 Origin;
        public ExampleNPCAgent(IDna Dna, int type) : base(Dna, type) { }

        public ExampleNPCAgent(int type) : base(type) { }

        public override IDna GenerateRandomAgent()
        {
            IDna network = new BaseNeuralNetwork(12)
                   .AddLayer<SigmoidActivationFunction>(12)
                   .SetOutput<SigmoidActivationFunction>(8)
                   .GenerateWeights(() => Main.rand.NextFloat(-2, 2));

            return network;
        }

        public override void CalculateCurrentFitness()
        {
            float distance = Math.Max(0.01f, Vector2.Distance(Main.LocalPlayer.position, Entity.position));
            float orgDistance = Math.Max(0.01f, Vector2.Distance(Origin, Main.LocalPlayer.position));

            float distFunc = (orgDistance / distance - 1);
            float target = 300f;


            //Fitness += Math.Max(0.01f, target * target - distance * distance) / (target * target);
            Fitness += (float)Math.Pow(target / Math.Max(0.01f, distance), MathHelper.E);
            Fitness *= 0.7f + (float)Math.Pow(20f / Math.Max(1, TimeInWall), 1.7f) / 70f;

            if (!Collision.CanHitLine(Entity.Center, 1, 1, Main.LocalPlayer.Center, 1, 1)) Fitness *= 0.3f;
            if(TimeToReach != 0) Fitness *= 1 + (float)Math.Pow(400f / Math.Max(1,TimeToReach), 1.7f) / 70f;
            Fitness *= 1 + (float)Math.Pow(NewPlaces * 0.8f, MathHelper.E) / 70f;
            Fitness = Math.Max(Fitness, 0);
        }

        public override void CalculateContinuousFitness()
        {
            float distance = Math.Max(0.1f, Vector2.Distance(Main.LocalPlayer.position, Entity.position));
            if(TimeAlive == 0)
            {
                Origin = Entity.position;
            }
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
                    if (Vector2.DistanceSquared(PlacesSeen[i], Entity.Center) < 40*40)
                    {
                        seen = true;
                        break;
                    }
                }
                if (!seen)
                {
                    NewPlaces++;
                    PlacesSeen.Add(Entity.Center);
                }
            }

            if (tilesAround >= 1 && Entity.velocity.Length() <= 0.5f) TimeStuck++;

            TimeInWall += tilesAround;

            if (distance <= 70f)
            {
                TimeToReach = TimeAlive;
            }

            if (TimeStuck > 30)
            {
                Kill();
                Fitness *= 0.5f;
            }
            
            //Fitness += (1f / distance) * 0.01f;
        }
    }
}