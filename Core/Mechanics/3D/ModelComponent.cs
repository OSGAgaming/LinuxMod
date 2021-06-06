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
        public Model currentModel { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public Effect Effect { get; set; }
        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
        public float Scale { get; set; }


        public ModelComponent(Model currentModelInput)
        {
            currentModel = currentModelInput;
            GraphicsDevice = Main.graphics.GraphicsDevice;
            Mechanic.GetMechanic<ModelHost>().modelComponents.Add(this);
            Effect = null;
        }

        public void Update() { }
        
        public void Unload()
        {
            Mechanic.GetMechanic<ModelHost>().modelComponents.Remove(this);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            Matrix world = 
                      Matrix.CreateRotationX(rotation.X)
                    * Matrix.CreateRotationY(rotation.Y)
                    * Matrix.CreateRotationZ(rotation.Z)
                    * Matrix.CreateScale(Scale)
                    * Matrix.CreateWorld(position, Vector3.UnitZ, Vector3.Up)
                    * Matrix.CreateTranslation(new Vector3(-width / 2, height / 2, 0)); //Move the models position

            // Compute camera matrices.
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), Vector3.Zero, Vector3.UnitY);

            //Create the 3D projection for this model
            Matrix projection = Matrix.CreateOrthographic(width, height, 0f, 1000f);

            Model model = LinuxMod.ModelManager.Planet;

            foreach (ModelMesh mesh in model.Meshes)
            {
                if (Effect != null)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        Effect effect = LinuxMod.ExampleModelShader;
                        part.Effect = effect;
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * world);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);

                    }
                }
                else
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = mesh.ParentBone.Transform * world;
                        effect.View = view;
                        effect.Projection = projection;
                    }
                }
                mesh.Draw();
            }
        }
    }
}