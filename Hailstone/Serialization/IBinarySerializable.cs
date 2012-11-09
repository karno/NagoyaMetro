using System.IO;

namespace Hailstone.Serialization
{
    public interface IBinarySerializable 
    {
        void Serialize(BinaryWriter writer);

        void Deserialize(BinaryReader reader);
    }
}
