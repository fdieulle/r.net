using System.Globalization;

namespace RDotNet.AssemblyTest
{
    public class Buffer
    {
        private static int instanceCounter;
        private readonly byte[] buffer;
        private readonly int instanceId;

        public Buffer(int count)
        {
            instanceId = ++instanceCounter;
            buffer = new byte[count];
        }

        public override string ToString()
        {
            return instanceId.ToString(CultureInfo.InvariantCulture);
        }

        public static Buffer Create(int count)
        {
            return new Buffer(count);
        }
    }
}
