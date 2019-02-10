using System.Text;

namespace Polly.Caching.Distributed.Specs.Integration
{
    public class ByteArraySerializer : ICacheItemSerializer<string, byte[]>
    {
        public string Deserialize(byte[] objectToDeserialize)
        {
            return objectToDeserialize == null ? null : Encoding.UTF8.GetString(objectToDeserialize);
        }

        public byte[] Serialize(string objectToSerialize)
        {
            return objectToSerialize == null ? null : Encoding.UTF8.GetBytes(objectToSerialize);
        }
    }
}
