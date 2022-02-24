using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace LinuxMod.Core.Mechanics
{
    public class EntityFocalCamera : CameraTransform
    {
        public Lintity FocalEntity { get; set; }

        public Vector2 Angle;
        public float MouseSensitivity = 0.01f;
        public float InternalWalkSpeed = 10;
        public bool isStatic;

        public EntityFocalCamera(
            Lintity entity, Vector3 cameraDirection, float fieldOfView = MathHelper.PiOver4,
            FrustrumType frustrum = FrustrumType.FOV, float nearPlane = 1f, float farPlane = 50000f, bool Static = false)

            : base(cameraDirection, fieldOfView, frustrum, nearPlane, farPlane)
        {
            FocalEntity = entity;
            Transform.Scale = -1;
            isStatic = Static;
        }


        protected override void OnUpdateTransform()
        {
            KeyboardState k = Keyboard.GetState();

            Vector2 directionX = new Vector2((float)Math.Sin(Angle.X), (float)Math.Cos(Angle.X));
            Vector2 directionY = new Vector2((float)Math.Sin(Angle.Y), (float)Math.Cos(Angle.Y));

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Angle.X += LinuxInput.DeltaMouse.X * MouseSensitivity;
                Angle.Y += LinuxInput.DeltaMouse.Y * MouseSensitivity;
            }

            Direction = Vector3.Normalize(new Vector3(directionX.X, directionY.X, directionX.Y * directionY.Y));

            if (FocalEntity != null)
                Transform.Position = FocalEntity.Transform.Position;
            else
            {
                if (!isStatic)
                {
                    if (k.IsKeyDown(Keys.W)) Transform.Position += new Vector3(Direction.X, 0, Direction.Z) * InternalWalkSpeed;
                    if (k.IsKeyDown(Keys.S)) Transform.Position -= new Vector3(Direction.X, 0, Direction.Z) * InternalWalkSpeed;

                    if (k.IsKeyDown(Keys.D)) Transform.Position += new Vector3(-Direction.Z, 0, Direction.X) * InternalWalkSpeed;
                    if (k.IsKeyDown(Keys.A)) Transform.Position -= new Vector3(-Direction.Z, 0, Direction.X) * InternalWalkSpeed;

                    if (k.IsKeyDown(Keys.Up)) Transform.Position += Vector3.UnitY * InternalWalkSpeed;
                    if (k.IsKeyDown(Keys.Down)) Transform.Position -= Vector3.UnitY * InternalWalkSpeed;
                }
            }
        }
    }
}
