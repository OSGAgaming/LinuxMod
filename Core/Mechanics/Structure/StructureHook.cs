using LinuxMod.Core.Assets;
using LinuxMod.Core.Helper.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LinuxMod.Core.Mechanics
{
    public class StructureState : Mechanic
    {
        public override void AddHooks()
        {
            On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
        }

        float XRot;
        float YRot;
        Vector2 Rot;
        private void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            GraphicsDevice GD = Main.graphics.GraphicsDevice;
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Vector3 Rotation = Vector3.Zero;
            float Scale = 1f;
            Vector3 Translation = new Vector3(new Vector2(Main.MouseScreen.X, -Main.MouseScreen.Y), -100);

            int width = GD.Viewport.Width;
            int height = GD.Viewport.Height;

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                XRot += 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                XRot -= 0.05f;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                YRot += 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                YRot -= 0.05f;

            Rot += (new Vector2(XRot, YRot) - Rot) / 32f;
            Matrix world = Matrix.CreateRotationX(Rot.X) 
                    * Matrix.CreateRotationY(Rot.Y)
                    * Matrix.CreateRotationZ(Rotation.Z)
                    * Matrix.CreateScale(Scale) 
                    * Matrix.CreateWorld(Translation, Vector3.UnitZ, Vector3.Up)
                    * Matrix.CreateTranslation(new Vector3(-width/2, height/2,0)); //Move the models position

            // Compute camera matrices.
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), Vector3.Zero, Vector3.UnitY);

            //Create the 3D projection for this model
            Matrix projection = Matrix.CreateOrthographic(width, height, 0f, 1000f);

            Model model = LinuxMod.ModelManager.Planet;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Effect effect = LinuxMod.ExampleModelShader;
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * world);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                }
                mesh.Draw();
            }
                  //      model.Draw(world, view, projection);

            Main.spriteBatch.End();
            orig(self);
        }
    }
}