using System;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class SexpIntegerDifftimeMatrixConverter : IConverter
    {
        private static readonly Type[] types = new[] {typeof (TimeSpan[,])};
        
        private readonly Matrix<int> sexpMatrix;
        private readonly string units;

        public SexpIntegerDifftimeMatrixConverter(Matrix<int> sexpMatrix)
        {
            this.sexpMatrix = sexpMatrix;
            this.units = sexpMatrix.GetUnits();
        } 

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return types;
        }

        public object Convert(Type type)
        {
            return sexpMatrix.ToArray().ToTimespan(units);
        }

        #endregion
    }

    public class SexpNumericDifftimeMatrixConverter : IConverter
    {
        private static readonly Type[] types = new[] { typeof(TimeSpan[,]) };

        private readonly Matrix<double> sexpMatrix;
        private readonly string units;

        public SexpNumericDifftimeMatrixConverter(Matrix<double> sexpMatrix)
        {
            this.sexpMatrix = sexpMatrix;
            this.units = sexpMatrix.GetUnits();
        }

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return types;
        }

        public object Convert(Type type)
        {
            return sexpMatrix.ToArray().ToTimespan(units);
        }

        #endregion
    }
}
