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
using LinuxMod.Core.Assets;

namespace LinuxMod.Core.Mechanics
{
    public class ReflectableModel : ModelComponent, IReflectable
    {
        public ReflectableModel(Model model, bool HasTexture = false, Effect effect = null) : base(model, HasTexture, effect) { }

        public void ReflectDraw(SpriteBatch spriteBatch, int Yplane)
        {
            Matrix world =
                      Matrix.CreateRotationX(Transform.Rotation.X)
                    * Matrix.CreateRotationY(Transform.Rotation.Y)
                    * Matrix.CreateRotationZ(Transform.Rotation.Z)
                    * Matrix.CreateScale(Transform.Scale)
                    * Matrix.CreateWorld(Transform.Position, Vector3.Forward, Vector3.Up); //Move the models position;

            // Get camera matrices.
            CameraTransform camera = DepthBuffer.GetLayer(Layer).Camera;

            Vector3 cameraPos = camera.Transform.Position;

            Vector3 ViewTransform = Vector3.Multiply(cameraPos, new Vector3(1, -1, 1));
            Vector3 ViewTarget = ViewTransform + Vector3.Multiply(camera.Direction, new Vector3(1, -1, 1));

            Matrix view = Matrix.CreateLookAt(ViewTransform, ViewTarget, Vector3.Up);
            Matrix projection = camera.ProjectionMatrix;

            LocalRenderer.GraphicsDeviceManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            LocalRenderer.GraphicsDeviceManager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            DiffusePointer = 0;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                if (Effect != null)
                {
                    foreach (Effect currentEffect in mesh.Effects)
                    {
                        ShaderParameters?.Invoke(currentEffect);

                        currentEffect.Parameters["matWorldViewProj"].
                           SetValue(mesh.ParentBone.Transform * world * view * projection);
                        currentEffect.Parameters["matWorld"].SetValue(mesh.ParentBone.Transform * world);
                        currentEffect.Parameters["vecEye"].SetValue(new Vector4(ViewTransform, -1));
                        currentEffect.Parameters["YCull"].SetValue(0);
                    }
                }
                else
                {
                    foreach (Effect effects in mesh.Effects)
                    {
                        if (effects is BasicEffect effect)
                        {
                            if (!HasTexture)
                                effect.EnableDefaultLighting();
                            else
                            {
                                try
                                {
                                    effect.Texture = Asset.GetTexture(Asset.ModelDirectory + TexturePath);
                                    effect.TextureEnabled = HasTexture;
                                }
                                catch
                                {
                                    effect.TextureEnabled = false;
                                }
                            }
                            effect.World = mesh.ParentBone.Transform * world;
                            effect.View = view;
                            effect.Projection = projection;
                            effect.FogEnabled = true;
                            effect.FogEnd = 4000f;
                            effect.FogStart = 2000f;
                            effect.PreferPerPixelLighting = true;
                            effect.FogColor = FogColor;
                        }
                    }
                }
                mesh.Draw();

            }
        }
    }
}