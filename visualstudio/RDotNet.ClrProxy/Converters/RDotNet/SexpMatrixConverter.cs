using System;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class SexpMatrixConverter<T> : IConverter
    {
        private static readonly Type[] types = new[] {typeof (T[,])};
        
        private readonly Matrix<T> sexpMatrix;

        public SexpMatrixConverter(Matrix<T> sexpMatrix)
        {
            this.sexpMatrix = sexpMatrix;
        } 

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return types;
        }

        public object Convert(Type type)
        {
            return sexpMatrix.ToArray();
        }

        #endregion
    }
}
