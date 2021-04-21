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
        float Viginette = 5;
        internal override void PlayerUpdate(Player player)
        {
            Viginette += (3 - Viginette) / 116f;

            Utils.ActivateScreenShader("Linux:Viginette");
            Utils.GetScreenShader("Linux:Viginette").UseIntensity(Viginette);

        }
    }
}