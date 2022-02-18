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
    public class ModelComponent : IComponent, ILayeredDraw
    {
        public Model Model { get; private set; }

        public Effect Effect { get; set; }

        public string Layer { get; set; }

        public Transform Transform;

        public bool HasTexture;
        public string TexturePath;
        public Action<Effect> ShaderParameters;

        public ModelComponent(Model currentModelInput, bool HasTexture = false)
        {
            Model model = currentModelInput;
            Model = model;
            this.HasTexture = HasTexture;

            Transform.Scale = 1;
            Effect = null;
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

            foreach (ModelMesh mesh in Model.Meshes)
            {

                if (Effect != null)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = Effect;
                        Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * world);
                        Effect.Parameters["View"].SetValue(view);
                        Effect.Parameters["Projection"].SetValue(projection);
                        ShaderParameters?.Invoke(Effect);
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
                            effect.FogColor = new Vector3(1);
                        }
                    }
                }
                mesh.Draw();

            }
        }
    }
}