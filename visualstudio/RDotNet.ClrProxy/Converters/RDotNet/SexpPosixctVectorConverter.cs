using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RDotNet.ClrProxy.Resources;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class SexpPosixctVectorConverter : IConverter
    {
        private static readonly Type[] multiValues = new[] { typeof(DateTime[]), typeof(List<DateTime>), typeof(IList<DateTime>), typeof(IEnumerable<DateTime>), typeof(Array), typeof(IEnumerable) };
        private static readonly Type[] singleValue = new[] {typeof (DateTime)}.Concat(multiValues).ToArray();

        private readonly Vector<double> sexpVector;
        private readonly Type[] types;
        private readonly TimeZoneInfo timezone;

        public SexpPosixctVectorConverter(Vector<double> sexpVector)
        {
            this.sexpVector = sexpVector;
            this.types = sexpVector.Length <= 1
                ? singleValue
                : multiValues;

            timezone = sexpVector.GetWindowsTimezone();
        }

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return this.types;
        }

        public object Convert(Type type)
        {
            if (type == typeof(DateTime))
                return sexpVector.ToArray().FromTick(timezone)[0];
            if (type == typeof(DateTime[]) || type == typeof(Array))
                return sexpVector.ToArray().FromTick(timezone);

            throw new InvalidOperationException(string.Format("Unexpected type on converter from R: {0} to .Net: {1}", sexpVector.Type, type));
        }

        #endregion
    }
}
