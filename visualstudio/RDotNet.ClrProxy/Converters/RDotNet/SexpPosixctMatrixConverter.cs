using System;
using RDotNet.ClrProxy.Resources;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class SexpPosixctMatrixConverter: IConverter
    {
        private static readonly Type[] types = new[] {typeof (DateTime[,])};
        
        private readonly Matrix<double> sexpMatrix;
        private readonly TimeZoneInfo timezone;

        public SexpPosixctMatrixConverter(Matrix<double> sexpMatrix)
        {
            this.sexpMatrix = sexpMatrix;
            timezone = sexpMatrix.GetWindowsTimezone();
        } 

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return types;
        }

        public object Convert(Type type)
        {
            return sexpMatrix.ToArray().FromTicks(timezone);
        }

        #endregion
    }
}
