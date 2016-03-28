using System;

namespace RDotNet.ClrProxy.Converters
{
    public interface IConverter
    {
        Type[] GetClrTypes();

        object Convert(Type type);
    }
}
