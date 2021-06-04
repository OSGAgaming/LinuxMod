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
    public class ModelRenderer
    {
        public AnimationPlayer animationPlayer; //Controls the animation, references a method in the pre-loaded project
        public AnimationClip clip; //Contains the animation clip currently playing
        public Model currentModel; //Model reference
        public SkinningData skinningData; //Used by the AnimationPlayer method
        public Vector3 Translation = new Vector3(0, 0, 0); //Fishes current position on the screen
        public Vector3 Rotation = new Vector3(0, 0, 0); //Current rotation
        public float Scale = 1.0f; //Current scale
        
        public void Update(GameTime gameTime)
        {
            if ((clip != null))
                animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
        }

        public ModelRenderer(Model currentModelInput)
        {
            currentModel = currentModelInput;
            // Look up our custom skinning information.
            skinningData = currentModel.Tag as SkinningData;
            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");
            // Create an animation player, and start decoding an animation clip.
            animationPlayer = new AnimationPlayer(skinningData);
        }

        public void ModelDraw(GraphicsDevice device, Vector3 cameraPosition, Vector3 cameraTarget, float farPlaneDistance)
        {
            Matrix[] bones = animationPlayer.GetSkinTransforms();
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i] *=
                      Matrix.CreateRotationX(Rotation.X) //Computes the rotation
                    * Matrix.CreateRotationY(Rotation.Y)
                    * Matrix.CreateRotationZ(Rotation.Z)
                    * Matrix.CreateScale(Scale) //Applys the scale
                    * Matrix.CreateWorld(Translation, Vector3.Forward, Vector3.Up); //Move the models position
            }

            float aspectRatio = device.Viewport.Width /
                                device.Viewport.Height;

            // Compute camera matrices.
            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Right);

            //Create the 3D projection for this model
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio,
                1.0f, farPlaneDistance);

            // Render the skinned mesh.
            foreach (ModelMesh mesh in currentModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["Bones"].SetValue(bones);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                }
                mesh.Draw();
            }
        }
    }
}