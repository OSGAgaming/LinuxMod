using System.IO;

namespace LinuxMod.Core.Mechanics.Interfaces
{
    public interface ISerializable
    {
        void Serialize(Stream stream);
    }
}
