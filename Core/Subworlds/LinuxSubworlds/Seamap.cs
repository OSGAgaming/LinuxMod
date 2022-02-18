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
using LinuxMod.Core.Mechanics.Primitives;

namespace LinuxMod.Core.Subworlds.LinuxSubworlds
{
    public class SeamapSubworld : Subworld
    {
        public static EntityFocalCamera BoatPOV;
        public LintitySet EntityCache;
        public SeamapSubworld()
        {
            /*
            EntityCache = new SeamapComponents();

            DepthSetCalls.Instance.CurrentLintitySet = EntityCache;
            EntityCache.OnActivate();

            BoatPOV = new EntityFocalCamera(null, Vector3.UnitZ);
            BoatPOV.Transform.Position.Y++;

            DepthBuffer.RegisterLayer(new CenterScisorLayer(0, new EntityFocalCamera(null, Vector3.UnitZ, Static: true, fieldOfView: MathHelper.PiOver4)), "Skybox");
            DepthBuffer.RegisterLayer(new CenterScisorLayer(1f, BoatPOV), "Seamap");
            */
        }

        public override Point Dimensions => new Point(500,500);

        public override Point SpawnTile => new Point(10,200);

        public override string Name => "Seamap";

        internal override void WorldGeneration()
        {

        }
        internal override void PlayerUpdate(Player player)
        {
            if(Main.GameUpdateCount == 0)
            {
                EntityCache = new SeamapComponents();

                DepthSetCalls.Instance.CurrentLintitySet = EntityCache;
                EntityCache.OnActivate();

                BoatPOV = new EntityFocalCamera(null, Vector3.UnitZ);
                BoatPOV.Transform.Position.Y++;

                DepthBuffer.RegisterLayer(new CenterScisorLayer(0, new EntityFocalCamera(null, Vector3.UnitZ, Static: true, fieldOfView: MathHelper.PiOver4)), "Skybox");
                DepthBuffer.RegisterLayer(new CenterScisorLayer(1f, BoatPOV), "Seamap");
            }
            
            DepthBuffer.GetLayer("Seamap").Camera.FarPlane = 10000f;

            player.invis = true;
            //player.AddBuff(BuffID.Cursed, 10);
            //player.AddBuff(BuffID.Frozen, 10);
            player.AddBuff(BuffID.PeaceCandle, 10);
            player.velocity = Vector2.Zero;
            player.gravity = 0;
            player.ZonePeaceCandle = true;
        }
    }

    public class SeamapComponents : LintitySet
    {
        public static SeamapWaterMesh Floor;
        public ReflectableModel Skybox;
        public ReflectableModel Globe;

        private readonly int FloorSize = 10000;

        public override void OnActivate()
        {
            Floor = new SeamapWaterMesh(
                 new Vector3(-FloorSize, 0, -FloorSize),
                 new Vector3(-FloorSize, 0, FloorSize),
                 new Vector3(FloorSize, 0, FloorSize),
                 new Vector3(FloorSize, 0, -FloorSize), Color.AliceBlue, 
                 "Seamap");

            //Drawables.Add(Floor);

            Skybox = new ReflectableModel(ModelLoader.SeamapSkybox, true);
            Skybox.Layer = "Skybox";
            Skybox.TexturePath = "CasualDay4K2";

            Globe = new ReflectableModel(ModelLoader.Planet, false);
            Globe.Layer = "Seamap";

            Drawables.Add(Skybox);
            Drawables.Add(Globe);
        }

        public override void Update()
        {
            LinuxMod.GetLoadable<SeamapWater>().AppendReflectionCall(Globe.ReflectDraw);
            LinuxMod.GetLoadable<SeamapWater>().AppendSkyboxCall(Skybox.ReflectDraw);

            Skybox.Transform.Scale = 1f;
            Skybox.Transform.Rotation.Y -= 0.0002f;
        }
    }
}