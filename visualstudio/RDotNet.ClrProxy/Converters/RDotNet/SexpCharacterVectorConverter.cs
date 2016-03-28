using System;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class SexpCharacterVectorConverter : IConverter
    {
        private static readonly Type[] singleValue = new[] { typeof(string), typeof(string[]) };
        private static readonly Type[] multiValues = new[] { typeof(string[]) };

        private readonly Vector<string> sexpVector;
        private readonly Type[] types;

        public SexpCharacterVectorConverter(Vector<string> sexpVector)
        {
            this.sexpVector = sexpVector;
            this.types = sexpVector.Length <= 1
                ? singleValue
                : multiValues;
        }

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return this.types;
        }

        public object Convert(Type type)
        {
            if (type == typeof(string))
                return sexpVector.ToArray()[0];
            if (type == typeof(string[]))
                return sexpVector.ToArray();
            if (type.IsEnum)
                return sexpVector.ToArray().ToEnum(type, true);
            if (type.IsEnumArray())
                return sexpVector.ToArray().ToEnum(type.GetElementType());

            throw new InvalidOperationException(string.Format("Unexpected type on converter from R: {0} to Clr: {1}", sexpVector.Type, type));
        }

        #endregion
    }
}
