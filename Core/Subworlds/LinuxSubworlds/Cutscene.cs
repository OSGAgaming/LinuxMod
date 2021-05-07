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
            LUtils.ClearRegion(100,8,new Vector2(10,200));
            LUtils.FillWall(500, 500, new Vector2(0, 0), WallID.BlueDungeon);
            //Utils.MakeCircleFromCenter();
        }
        internal override void PlayerUpdate(Player player)
        {
            if (Main.GameUpdateCount == 3)
            {
                //LiquidRender.Instance.liquidHost.AddLiquid(new Rectangle(42 * 16, 205 * 16, 500, 100), 50, 0.04f, 0.05f);
                Gloop liquid = new Gloop
                {
                    frame = new Rectangle(42 * 16 + 500, 207 * 16, 1000, 16),
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