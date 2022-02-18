using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinuxMod.Core.Mechanics.Interfaces
{
    public interface IReflectable
    {
        void ReflectDraw(SpriteBatch sb, int YPlane);
    }
}
