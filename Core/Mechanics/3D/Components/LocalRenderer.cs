using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace LinuxMod.Core.Mechanics
{
    public static class LocalRenderer
    {
        public static Point MaxResolution => new Point(2560, 1440);
        public static Rectangle MaxResolutionBounds => new Rectangle(0, 0, MaxResolution.X, MaxResolution.Y);

        public static GraphicsDeviceManager GraphicsDeviceManager => Main.graphics;

        public static GraphicsDevice Device => GraphicsDeviceManager.GraphicsDevice;

        public static Viewport Viewport => Device.Viewport;

        public static Point ViewportSize => new Point(Viewport.Width, Viewport.Height);

        public static PresentationParameters PresentationParameters => Device.PresentationParameters;

        public static Point BackBufferSize => new Point(PresentationParameters.BackBufferWidth, PresentationParameters.BackBufferHeight);

        public static EntityFocalCamera DefaultCamera { get; set; }

        public static CameraTransform UICamera { get; set; }

        public static void Load()
        {
            DepthBuffer.Load();

            InitializeCameras();
            RegisterLayers();
        }

        public static void InitializeCameras()
        {
            DefaultCamera = new EntityFocalCamera(null, Vector3.UnitZ, fieldOfView: MathHelper.PiOver2 * 1.2f, farPlane: 10000);
        }

        public static void RegisterLayers()
        {     
            DepthBuffer.RegisterLayer(new CenterScisorLayer(0, DefaultCamera), "Default");
            DepthBuffer.RegisterLayer(new DepthLayer(2, new CameraTransform(Vector3.UnitZ)), "Logger");
        }

        public static void Unload()
        {
            DepthBuffer.Unload();
            DefaultCamera = null;
            UICamera = null;
        }
        

        public static void DrawSceneToTarget(LintitySet scene, SpriteBatch sb)
        {
            if (scene != null)
            {
                DepthBuffer.DrawLayersToTarget(scene, sb);
                DepthBuffer.DrawLayers(sb);
            }
        }
    }
}
