using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class SexpCharacterVectorConverter : IConverter
    {
        private static readonly Type[] multiValues = new[] { typeof(string[]), typeof(List<string>), typeof(IList<string>), typeof(IEnumerable<string>), typeof(Array), typeof(IEnumerable) };
        private static readonly Type[] singleValue = new[] {typeof (string)}.Concat(multiValues).ToArray();
        
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
            if (type == typeof(string[]) || type == typeof(Array) || type == typeof(IEnumerable<string>) || type == typeof(IEnumerable))
                return sexpVector.ToArray();
            if (type == typeof (List<string>) || type == typeof (IList<string>))
                return sexpVector.ToArray().ToList();
            if (type.IsEnum)
                return sexpVector.ToArray().ToEnum(type, true);
            if (type.IsEnumArray())
                return sexpVector.ToArray().ToEnum(type.GetElementType());

            throw new InvalidOperationException(string.Format("Unexpected type on converter from R: {0} to Clr: {1}", sexpVector.Type, type));
        }

        #endregion
    }
}
