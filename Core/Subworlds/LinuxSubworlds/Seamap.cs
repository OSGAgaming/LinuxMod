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
            if (Main.GameUpdateCount == 0)
            {
                EntityCache = new SeamapComponents();

                DepthSetCalls.Instance.CurrentLintitySet = EntityCache;
                EntityCache.OnActivate();

                BoatPOV = new EntityFocalCamera(null, Vector3.UnitZ);
                BoatPOV.Transform.Position.Y++;
                BoatPOV.InternalWalkSpeed = 40;

                DepthBuffer.RegisterLayer(new CenterScisorLayer(0, new EntityFocalCamera(null, Vector3.UnitZ, Static: true, fieldOfView: MathHelper.PiOver4), LinuxTechTips.GetScreenShader("PixelationShader").Shader), "Skybox");
                DepthBuffer.RegisterLayer(new CenterScisorLayer(1f, BoatPOV, LinuxTechTips.GetScreenShader("PixelationShader").Shader), "Seamap");
                DepthBuffer.RegisterLayer(new DepthLayer(2f, BoatPOV, LinuxTechTips.GetScreenShader("PixelationShader").Shader), "Water");
            }
        }

        public override Point Dimensions => new Point(500,500);

        public override Point SpawnTile => new Point(10,200);

        public override string Name => "Seamap";

        internal override void WorldGeneration()
        {

        }
        internal override void PlayerUpdate(Player player)
        {           
            DepthBuffer.GetLayer("Seamap").Camera.FarPlane = 40000f;
            BoatPOV.InternalWalkSpeed = 1;

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
        public static WaterMesh3D Floor;
        public static WaterMeshContainer3D WaterContainer; 
        public ReflectableModel Skybox;
        public ReflectableModel Globe;
        public ReflectableModel Lake;

        private readonly int FloorSize = 1000;

        public override void OnActivate()
        {
            //==================Initialize Section=====================

            Skybox = new ReflectableModel(ModelLoader.SeamapSkybox, true);
            Skybox.Layer = "Skybox";
            Skybox.TexturePath = "CasualDay4K2";

            Globe = new ReflectableModel(ModelLoader.Planet, false);
            Globe.Layer = "Seamap";

            Lake = new ReflectableModel(ModelLoader.Mountains, false, LinuxTechTips.GetScreenShader("NormalMapModelShader").Shader);
            Lake.Layer = "Seamap";

            Lake.ShaderParameters = (effect) =>
            {
                effect.Parameters["vecEye"].SetValue(new Vector4(DepthBuffer.GetLayer(Lake.Layer).Camera.Transform.Position, 1));
                effect.Parameters["YCull"].SetValue(-int.MaxValue);
                effect.Parameters["FogMap"].SetValue(DepthBuffer.GetLayer("Skybox").Target);
                effect.Parameters["FogStart"].SetValue(500);
                effect.Parameters["FogEnd"].SetValue(1000);
                if (Lake.Colors.Count > 0)
                    effect.Parameters["vAmbient"].SetValue(Lake.Colors[Lake.DiffusePointer++]);
            };

            WaterContainer = new WaterMeshContainer3D();
            WaterContainer.Load();

            Floor = new WaterMesh3D(
              new Vector3(-FloorSize, 0, -FloorSize),
              new Vector3(-FloorSize, 0, FloorSize),
              new Vector3(FloorSize, 0, FloorSize),
              new Vector3(FloorSize, 0, -FloorSize), WaterContainer, Color.AliceBlue,
              "Seamap");

            //==================Set Transforms and Properties Section=====================
            Lake.Transform.Scale = 300f;
            Lake.Transform.Position.Y = -200f;
            Lake.FogColor = new Color(94, 94, 110).ToVector3();

            //==================Drawables=====================
            Drawables.Add(Skybox);
            Drawables.Add(Globe);
            Drawables.Add(Lake);
            Drawables.Add(WaterContainer);

            //==================Misc Section=====================
            DepthSetCalls.OnPreDraw += WaterContainer.DrawOcclusionMap;
            DepthSetCalls.OnPreDraw += WaterContainer.RenderWaterOcclusion;
            DepthSetCalls.OnPreUpdate += WaterContainer.ClearCalls;
        }

        public override void Update()
        {
            LinuxTechTips.GetScreenShader("PixelationShader").Shader.Parameters["accuracy"].SetValue(3);

            WaterContainer.AppendReflectionCall(Globe.ReflectDraw);
            WaterContainer.AppendReflectionCall(Lake.ReflectDraw);
            WaterContainer.AppendSkyboxCall(Skybox.ReflectDraw);     

            Globe.Transform.Position.Y -= 10;
            Lake.Transform.Scale = 10f;
            Lake.Transform.Position.Y = 50f;

            Skybox.Transform.Scale = 1f;
            Skybox.Transform.Rotation.Y -= 0.0002f;
        }
    }
}