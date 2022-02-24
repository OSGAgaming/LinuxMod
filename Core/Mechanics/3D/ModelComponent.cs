using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public class ModelComponent : IComponent, ILayeredDraw
    {
        public Model Model { get; private set; }

        public Effect Effect { get; private set; }

        public string Layer { get; set; }

        public int DiffusePointer { get; set; }

        public Transform Transform;

        public bool HasTexture;
        public string TexturePath;
        public Action<Effect> ShaderParameters;
        public Vector3 FogColor;
        public float YCull = -float.MaxValue;
        public List<Vector3> Colors;

        public ModelComponent(Model currentModelInput, bool HasTexture = false, Effect effect = null)
        {
            Model model = currentModelInput;
            Model = model;
            this.HasTexture = HasTexture;

            Transform.Scale = 1;
            Effect = effect;
            Colors = new List<Vector3>();

            if (effect == null)
                return;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (Effect effects in mesh.Effects)
                {
                    if (effects is BasicEffect _effect)
                    {
                        Colors.Add(_effect.DiffuseColor);
                    }
                }
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = Effect.Clone();
            }
        }

        public void Update() { }

        public void Draw(SpriteBatch spriteBatch)
        {
            Matrix world =
                      Matrix.CreateRotationX(Transform.Rotation.X)
                    * Matrix.CreateRotationY(Transform.Rotation.Y)
                    * Matrix.CreateRotationZ(Transform.Rotation.Z)
                    * Matrix.CreateScale(Transform.Scale)
                    * Matrix.CreateWorld(Transform.Position, Vector3.Forward, Vector3.Up); //Move the models position

            // Get camera matrices.
            CameraTransform camera = DepthBuffer.GetLayer(Layer).Camera;

            Matrix view = camera.ViewMatrix;
            Matrix projection = camera.ProjectionMatrix;

            LocalRenderer.GraphicsDeviceManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            DiffusePointer = 0;
            foreach (ModelMesh mesh in Model.Meshes)
            {
                if (Effect != null)
                {
                    foreach (Effect currentEffect in mesh.Effects)
                    {
                        currentEffect.Parameters["matWorldViewProj"]?.SetValue(mesh.ParentBone.Transform * world * view * projection);
                        currentEffect.Parameters["matWorld"]?.SetValue(mesh.ParentBone.Transform * world);

                        ShaderParameters?.Invoke(currentEffect);
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
                            effect.FogStart = 1000f;
                            effect.FogColor = FogColor;
                            effect.PreferPerPixelLighting = true;
                        }
                    }
                }
                mesh.Draw();

            }
        }
    }
}