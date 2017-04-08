using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class SexpIntegerDifftimeVectorConverter : IConverter
    {
        private static readonly Type[] multiValues = new[] { typeof(TimeSpan[]), typeof(List<TimeSpan>), typeof(IList<TimeSpan>), typeof(IEnumerable<TimeSpan>), typeof(Array), typeof(IEnumerable) };
        private static readonly Type[] singleValue = new[] {typeof (TimeSpan)}.Concat(multiValues).ToArray();
        
        private readonly Vector<int> sexpVector;
        private readonly Type[] types;
        private readonly string units;

        public SexpIntegerDifftimeVectorConverter(Vector<int> sexpVector)
        {
            this.sexpVector = sexpVector;
            this.types = sexpVector.Length <= 1
                ? singleValue
                : multiValues;
            this.units = this.sexpVector.GetUnits();
        }

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return this.types;
        }

        public object Convert(Type type)
        {
            if (type == typeof (TimeSpan))
                return sexpVector.ToArray().ToTimespan(units)[0];
            if (type == typeof(TimeSpan[]) || type == typeof(Array) || type == typeof(IEnumerable))
                return sexpVector.ToArray().ToTimespan(units);
            if (type == typeof (List<TimeSpan>) || type == typeof (IList<TimeSpan>) || type == typeof (IEnumerable<TimeSpan>))
                return sexpVector.ToArray().ToTimespan(units).ToList();

            throw new InvalidOperationException(string.Format("Unexpected type on converter from R: {0} to .Net: {1}", sexpVector.Type, type));
        }

        #endregion
    }

    public class SexpNumericDifftimeVectorConverter : IConverter
    {
        private static readonly Type[] singleValue = new[] { typeof(TimeSpan[]), typeof(TimeSpan) };
        private static readonly Type[] multiValues = new[] { typeof(TimeSpan[]) };

        private readonly Vector<double> sexpVector;
        private readonly Type[] types;
        private readonly string units;

        public SexpNumericDifftimeVectorConverter(Vector<double> sexpVector)
        {
            this.sexpVector = sexpVector;
            this.types = sexpVector.Length <= 1
                ? singleValue
                : multiValues;
            this.units = this.sexpVector.GetUnits();
        }

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return this.types;
        }

        public object Convert(Type type)
        {
            if (type == typeof(TimeSpan))
                return sexpVector.ToArray().ToTimespan(units)[0];
            if (type == typeof(TimeSpan[]))
                return sexpVector.ToArray().ToTimespan(units);

            throw new InvalidOperationException(string.Format("Unexpected type on converter from R: {0} to .Net: {1}", sexpVector.Type, type));
        }

        #endregion
    }
}
