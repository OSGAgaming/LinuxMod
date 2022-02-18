
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

                CollisionInfo InfoInf = CollisionInfo.Default;
                Vector2 TotalComponent = Vector2.Zero;
                int TotalCollisions = 0;

                foreach (PolygonModule pm in m)
                {
                    Vector2 p = Main.LocalPlayer.position;

                    Polygon polyInf = Polygon.RectToPoly(new Rectangle((int)p.X - 2, (int)p.Y - 2, Main.LocalPlayer.width + 4, Main.LocalPlayer.height + 4));
                    Polygon poly = Polygon.RectToPoly(new Rectangle((int)p.X, (int)p.Y, Main.LocalPlayer.width, Main.LocalPlayer.height));

                    CollisionInfo Info = LinuxCollision.SAT(poly, pm.Polygon);
                    InfoInf = LinuxCollision.SAT(polyInf, pm.Polygon);
                    if (InfoInf.d != Vector2.Zero)
                    {
                        TotalComponent += InfoInf.Axis;
                        TotalCollisions++;
                    }

                    resolve += Info.d;
                    walkSpace += InfoInf.d;
                }

                if (walkSpace != Vector2.Zero)
                {
                    if (TotalCollisions != 0)
                    {
                        TotalComponent /= TotalCollisions;
                        resolve *= new Vector2(Math.Abs(TotalComponent.X), Math.Abs(TotalComponent.Y));

                        if (Main.LocalPlayer.controlLeft || Main.LocalPlayer.controlRight) TotalComponent.Y = 1;
                        else TotalComponent.Y = Math.Abs(TotalComponent.Y);

                        if (Main.LocalPlayer.controlJump) TotalComponent.X = 1;
                        else TotalComponent.X = Math.Abs(TotalComponent.X);

                        Vector2 ParralelComponent = new Vector2(TotalComponent.Y, TotalComponent.X);
                        Main.LocalPlayer.velocity = Main.LocalPlayer.velocity * ParralelComponent;
                        Main.NewText(ParralelComponent);
                    }

                    Main.LocalPlayer.Center -= resolve;
                    Main.LocalPlayer.gravity = 0;
                }
            }
        }
        public override void PreUpdate()
        {
            LinuxMod.GetLoadable<InsigniaHost>().Update();
        }
    }
}
