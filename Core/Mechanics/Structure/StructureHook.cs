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

        private void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            GraphicsDevice GD = Main.graphics.GraphicsDevice;
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
/*
            float aspectRatio = GD.Viewport.Width / GD.Viewport.Height;
            Vector2 zoom = Main.GameViewMatrix.Zoom;

            Vector3 Rotation = Vector3.Zero;
            float Scale = 1f;
            Vector3 Translation = new Vector3(Main.MouseScreen, 1.5f);

            Matrix world = Matrix.CreateRotationX(Rotation.X) 
                    * Matrix.CreateRotationY(Rotation.Y)
                    * Matrix.CreateRotationZ(Rotation.Z)
                    * Matrix.CreateScale(Scale) 
                    * Matrix.CreateWorld(Translation, Vector3.Forward, Vector3.Up); //Move the models position

            // Compute camera matrices.
            Matrix view = Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);

            //Create the 3D projection for this model
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(179f), aspectRatio, 1f, 2f);

            ModelLoader.model.Draw(world, view, projection);
*/
            Main.spriteBatch.End();
            orig(self);
        }
    }
}