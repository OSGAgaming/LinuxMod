using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using SkinnedModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Reflection;

namespace LinuxMod.Core.Mechanics
{

    public enum FrustrumType
    {
        Regular,
        FOV,
        OffCenter
    }

    public class CameraTransform : Lintity
    {
        public Vector3 Direction;
        public float FieldOfView;
        public float NearPlane;
        public float FarPlane;

        public FrustrumType ProjectionType;

        public CameraTransform(Vector3 direction, float fieldOfView = MathHelper.PiOver2, FrustrumType frustrum = FrustrumType.FOV, float nearPlane = .1f, float farPlane = 5000f, float scale = 1)
        {
            Direction = direction;
            FieldOfView = fieldOfView;
            NearPlane = nearPlane;
            FarPlane = farPlane;
            Transform.Scale = scale;

            ProjectionType = frustrum;
        }

        public Matrix TransformationMatrix { get; set; }

        public Vector3 Target => Transform.Position + Direction;

        public Matrix ViewMatrix => Matrix.CreateLookAt(Transform.Position, Target, Vector3.Up);

        public Matrix WorldMatrix => Matrix.CreateScale(Transform.Scale);

        public Matrix ProjectionMatrix
        {
            get
            {
                switch (ProjectionType)
                {
                    case FrustrumType.Regular:
                        return Matrix.CreatePerspective(1, (float)LocalRenderer.MaxResolution.Y / LocalRenderer.MaxResolution.X, NearPlane, FarPlane);

                    case FrustrumType.FOV:
                        return Matrix.CreatePerspectiveFieldOfView(FieldOfView, LocalRenderer.BackBufferSize.X / (float)LocalRenderer.BackBufferSize.Y, NearPlane, FarPlane);
                    //TODO: do this one :P
                    case FrustrumType.OffCenter:
                        return Matrix.CreatePerspectiveOffCenter(0, 0, 0, 0, NearPlane, FarPlane);
                }

                return Matrix.Identity;
            }
        }


        protected virtual void OnUpdateTransform() { }

        protected virtual void TransformConfiguration()
        {
            TransformationMatrix =
            Matrix.CreateTranslation(-Transform.Position) *
            Matrix.CreateRotationZ(Transform.Rotation.Z) *
            Matrix.CreateScale(GetScreenScale());
        }

        public void Update()
        {
            TransformConfiguration();
            OnUpdateTransform();
        }

        public Vector3 GetScreenScale()
        {
            float scaleX = 1;
            float scaleY = 1;

            return new Vector3(scaleX * Transform.Scale, scaleY * Transform.Scale, 1.0f);
        }
    }
}