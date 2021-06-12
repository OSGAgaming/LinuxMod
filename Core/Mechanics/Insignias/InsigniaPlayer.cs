
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;
using LinuxMod.Core.Subworlds;
using LinuxMod;
using LinuxMod.Core.Subworlds.LinuxSubworlds;
using LinuxMod.Core.Mechanics;

namespace LinuxMod.Core
{
    public class InsigniaPlayer : ModPlayer
    {
        public bool Invisible = false;

        public override void UpdateBiomeVisuals()
        {
            if (Module.GetHost<PolygonModule>() != null)
            {
                IList<PolygonModule> m = Module.GetHost<PolygonModule>().Objects;

                Vector2 resolve = Vector2.Zero;
                Vector2 walkSpace = Vector2.Zero;
                foreach (PolygonModule pm in m)
                {
                    Vector2 p = Main.LocalPlayer.position;

                    Polygon polyInf = Polygon.RectToPoly(new Rectangle((int)p.X - 2, (int)p.Y - 2, Main.LocalPlayer.width + 4, Main.LocalPlayer.height + 4));
                    Polygon poly = Polygon.RectToPoly(new Rectangle((int)p.X, (int)p.Y, Main.LocalPlayer.width, Main.LocalPlayer.height));

                    CollisionInfo Info = LinuxCollision.SAT(poly, pm.Polygon);
                    CollisionInfo InfoInf = LinuxCollision.SAT(polyInf, pm.Polygon);

                    resolve += Info.d;
                    walkSpace += InfoInf.d;
                }

                if (walkSpace != Vector2.Zero)
                {
                    Main.LocalPlayer.Center -= resolve;
                    Main.LocalPlayer.velocity.Y = 0;
                    Main.LocalPlayer.gravity = 0;
                }
            }
        }
        public override void PreUpdate()
        {
            LinuxMod.InsigniaSystem.Update();
        }
    }
}
