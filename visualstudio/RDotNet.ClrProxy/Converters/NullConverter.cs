using System;

namespace RDotNet.ClrProxy.Converters
{
    public class NullConverter : IConverter
    {
        public static readonly Type[] Types = new [] { typeof(void) };
        public static readonly IConverter Instance = new NullConverter();

        private NullConverter(){}

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return Types;
        }

        public object Convert(Type type)
        {
            return null;
        }

        #endregion
    }
}
