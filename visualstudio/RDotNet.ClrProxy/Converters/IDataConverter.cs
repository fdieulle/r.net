using System;

namespace RDotNet.ClrProxy.Converters
{
    public interface IDataConverter
    {
        IConverter GetConverter(long address);

        object ConvertBack(Type type, object data);
    }
}
