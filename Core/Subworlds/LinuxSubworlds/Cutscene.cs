using LinuxMod.Core.Assets;
using LinuxMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core.Helper;
using LinuxMod.Core.Mechanics.Verlet;
using Microsoft.Xna.Framework.Input;

namespace LinuxMod.Core.Subworlds.LinuxSubworlds
{
    public class CutsceneSubworld : Subworld
    {
        public override Point Dimensions => new Point(500,500);

        public override Point SpawnTile => new Point(10,200);

        public override string Name => "Cutscene";

        internal override void WorldGeneration()
        {
            LUtils.FillRegion(500, 500, new Vector2(0,0), TileID.BlueDungeonBrick);
            LUtils.ClearRegion(172,8,new Vector2(10,200));
            for (int i = 2; i < 10; i++)
            {
                if(WorldGen.InWorld(20 * i, 203))
                WorldGen.PlaceTile(20 * i, 203, TileID.Torches);
            }
            LUtils.FillWall(500, 500, new Vector2(0, 0), WallID.BlueDungeon);
            //Utils.MakeCircleFromCenter();
        }
        internal override void PlayerUpdate(Player player)
        {
            if (Main.GameUpdateCount == 3)
            {
                LinuxMod.verletSystem.BindPoints(new VPoint[]
                { new VPoint(Main.LocalPlayer.Center, true),
                  new VPoint(Main.LocalPlayer.Center + new Vector2(10, 10), false),
                  new VPoint(Main.LocalPlayer.Center + new Vector2(0, 20), false)
                });
                //LiquidRender.Instance.liquidHost.AddLiquid(new Rectangle(42 * 16, 205 * 16, 500, 100), 50, 0.04f, 0.05f);
                Gloop liquid = new Gloop
                {
                    frame = new Rectangle(16*72, 207 * 16, 1600, 16),
                    accuracy = 50,
                    viscosity = 0.07f,
                    constant = 50,
                    dampening = 0.07f,
                    color = Color.LightBlue
                };

                LiquidRender.Instance.liquidHost.AddLiquid(liquid);
            }
        }
    }
}