using LinuxMod.Core.Assets;
using LinuxMod.Core.Helper.Extensions;
using LinuxMod.Core.Subworlds;
using LinuxMod.Core.Subworlds.LinuxSubworlds;
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

        float ScaleVel;
        float ScaleVel2;

        bool Enlarging = false;
        int delay;

        ModelComponent modelComponent = new ModelComponent(ModelLoader.Planet);
        ModelComponent clouds = new ModelComponent(ModelLoader.Clouds, false, LinuxMod.ExampleModelShader);

        private void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            if (delay > 0) delay--;

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                XRot += 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                XRot -= 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                YRot += 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                YRot -= 0.05f;

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && delay == 0)
            {
                //SubworldManager.EnterSubworld<SeamapSubworld>();

                Enlarging = !Enlarging;
                delay = 60;
            }

            Rot += (new Vector2(XRot, YRot) - Rot) / 32f;

            modelComponent.Transform.Position = new Vector3(Main.LocalPlayer.Center + new Vector2(60, 0).ForDraw(), -100);
            modelComponent.Transform.Rotation = new Vector3(Rot, 0);

            clouds.Transform.Position = new Vector3(Main.LocalPlayer.Center + new Vector2(60, 0).ForDraw(), -100);
            clouds.Transform.Rotation = new Vector3(Rot, 0);

            if (Enlarging) ScaleVel += (0.3f - modelComponent.Transform.Scale) / 30f - ScaleVel / 7f;
            else ScaleVel += (0f - modelComponent.Transform.Scale) / 80f - ScaleVel / 7f;

            if (Enlarging) ScaleVel2 += (0.4f - clouds.Transform.Scale) / 30f - ScaleVel2 / 7f;
            else ScaleVel2 += (0f - clouds.Transform.Scale) / 80f - ScaleVel2 / 7f;

            modelComponent.Transform.Scale += ScaleVel;
            clouds.Transform.Scale += ScaleVel2;

            clouds.ShaderParameters = (effect) =>
            {
                effect.Parameters["Progress"].SetValue(Main.GameUpdateCount);
                effect.Parameters["noiseTexture"].SetValue(Asset.GetTexture("Noise/noise"));
            };

            modelComponent.Draw(Main.spriteBatch);
            clouds.Draw(Main.spriteBatch);

     

            Main.spriteBatch.End();
            orig(self);
        }
    }
}