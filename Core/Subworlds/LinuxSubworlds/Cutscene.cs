using LinuxMod.Core.Assets;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Subworlds.LinuxSubworlds
{
    public class CutsceneSubworld : Subworld
    {
        public override Point Dimensions => new Point(500,500);

        public override Point SpawnTile => new Point(10,200);

        public override string Name => "Cutscene";

        internal override void WorldGeneration()
        {
            Utils.FillRegion(500, 500, new Vector2(0,0), TileID.BlueDungeonBrick);
            Utils.ClearRegion(100,8,new Vector2(10,200));
            Utils.FillWall(500, 500, new Vector2(0, 0), WallID.BlueDungeon);
            //Utils.MakeCircleFromCenter();
        }
        internal override void PlayerUpdate(Player player)
        {
            if (Main.GameUpdateCount == 3)
            {
                //LiquidRender.Instance.liquidHost.AddLiquid(new Rectangle(42 * 16, 205 * 16, 500, 100), 50, 0.04f, 0.05f);
                LiquidRender.Instance.liquidHost.AddLiquid(new Rectangle(42 * 16 + 500, 207 * 16, 1000, 16), 50, 0.07f, 0.07f);
            }

            Utils.Particles.SetSpawningModules(new SpawnRandomly(0.03f));
            Utils.Particles.SpawnParticles(player.Center, Vector2.Zero);
            Utils.Particles.SpawnParticles(player.Center, -Vector2.UnitY, 3, 
                null, new SlowDown(0.98f), new RotateTexture(Main.rand.NextFloat(-0.03f, 0.03f)), 
                new SetMask(Asset.GetTexture("Masks/RadialGradient")), new AfterImageTrail(1f), 
                new RotateVelocity(Main.rand.NextFloat(-0.02f, 0.02f)), new SetLighting(Color.White.ToVector3(), 0.1f));
        }
    }
}