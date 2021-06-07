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

namespace LinuxMod.Core.Mechanics
{
    public class ModelComponent : IComponent
    {
        public Model currentModel;
        public GraphicsDevice GraphicsDevice { get; set; }
        public Effect Effect { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public float Scale { get; set; }

        public Action<Effect> ShaderParameters;

        public ModelComponent(Model currentModelInput)
        {
            Model model = currentModelInput;
            currentModel = model;
            GraphicsDevice = Main.graphics.GraphicsDevice;
            Scale = 1f;
            Effect = null;
        }

        public void Update() { }
        
        public void Unload()
        {
            Mechanic.GetMechanic<ModelHost>().modelComponents.Remove(this);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ModelHost.SubscribeCall((sb) =>
            {
                int width = GraphicsDevice.Viewport.Width;
                int height = GraphicsDevice.Viewport.Height;

                Matrix world =
                          Matrix.CreateRotationX(Rotation.X)
                        * Matrix.CreateRotationY(Rotation.Y)
                        * Matrix.CreateRotationZ(Rotation.Z)
                        * Matrix.CreateScale(Scale/2)
                        * Matrix.CreateWorld(new Vector3(Position.X / 2, -Position.Y / 2, Position.Z), Vector3.UnitZ, Vector3.Up)
                        * Matrix.CreateTranslation(new Vector3(-width / 2, height / 2, 0)); //Move the models position

                // Compute camera matrices.
                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), Vector3.Zero, Vector3.UnitY);

                //Create the 3D projection for this model
                Matrix projection = Matrix.CreateOrthographic(width, height, 0f, 1000f);

                Model model = currentModel;

                foreach (ModelMesh mesh in model.Meshes)
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
                                effect.World = mesh.ParentBone.Transform * world;
                                effect.View = view;
                                effect.Projection = projection;
                                effect.EnableDefaultLighting();
                                effect.SpecularPower = 100;
                                effect.AmbientLightColor = Microsoft.Xna.Framework.Color.DeepSkyBlue.ToVector3();
                                effect.SpecularColor = Microsoft.Xna.Framework.Color.Black.ToVector3();
                            }
                        }
                    }
                    mesh.Draw();
                }
            });  
        }
    }
}