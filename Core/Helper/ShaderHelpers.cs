
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using System.Diagnostics;
//using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Terraria.DataStructures;

using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace LinuxMod.Core
{
    public static partial class Utils
    {
        public static void ActivateScreenShader(string ShaderName, Vector2 vec = default)
        {
            if (Main.netMode != NetmodeID.Server && !Filters.Scene[ShaderName].IsActive())
            {
                Filters.Scene.Activate(ShaderName, vec).GetShader();
            }
        }

        public static ScreenShaderData GetScreenShader(string ShaderName) => Filters.Scene[ShaderName].GetShader();
        
    }
}