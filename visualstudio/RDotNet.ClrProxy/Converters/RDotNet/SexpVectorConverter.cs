using System;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class SexpVectorConverter<T> : IConverter
    {
        private static readonly Type[] singleValue = new[] { typeof(T), typeof(T[]) };
        private static readonly Type[] multiValues = new[] { typeof(T[]) };

        private readonly Vector<T> sexpVector;
        private readonly Type[] types;

        public SexpVectorConverter(Vector<T> sexpVector)
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
            if (type == typeof(T))
                return sexpVector.ToArray()[0];
            if (type == typeof(T[]))
                return sexpVector.ToArray();

            throw new InvalidOperationException(string.Format("Unexpected type on converter from R: {0} to Clr: {1}", sexpVector.Type, type));
        }

        #endregion
    }
}
