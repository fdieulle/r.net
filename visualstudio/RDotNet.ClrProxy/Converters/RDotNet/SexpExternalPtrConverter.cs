using System;
using System.Runtime.InteropServices;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class SexpExternalPtrConverter : IConverter, IDisposable
    {
        private const string NET_OBJ_TAG = ".NetObj";

        private readonly SymbolicExpression sexp;
        private readonly Type[] types;

        public SexpExternalPtrConverter(SymbolicExpression sexp)
        {
            this.sexp = sexp;

            var tagPtr = sexp.Engine.GetFunction<R_ExternalPtrTag>()(sexp.DangerousGetHandle());
            var tag = sexp.Engine.CreateFromNativeSexp(tagPtr).AsCharacter().ToArray();

            if (tag == null || tag.Length != 2)
                throw new InvalidOperationException("This external pointer isn't supported");

            if (string.Equals(tag[0], NET_OBJ_TAG))
            {
                var typeStr = tag[1];

                Type type;
                string errorMsg;

                types = typeStr.TryGetType(out type, out errorMsg)
                            ? type.GetFullHierarchy()
                            : new[] {typeof (object)};
            }
            else
            {
                throw new InvalidOperationException(string.Format("This external pointer isn't supported, the tag should starts with {0} value", NET_OBJ_TAG));   
            }
        }

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return types;
        }

        public object Convert(Type type)
        {
            var objptr = sexp.Engine.GetFunction<R_ExternalPtrAddr>()(sexp.DangerousGetHandle());
            return Marshal.GetObjectForIUnknown(objptr);
        }

        #endregion

        public static SymbolicExpression ConvertBack(REngine engine, object data)
        {
            var tag = engine.CreateCharacterVector(new[] { NET_OBJ_TAG, data.GetType().FullName });
            var ptr = engine.GetFunction<R_MakeExternalPtr>()(
                Marshal.GetIUnknownForObject(data),
                tag.DangerousGetHandle(),
                engine.NilValue.DangerousGetHandle());

            return engine.CreateFromNativeSexp(ptr);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            var objPtr = sexp.Engine.GetFunction<R_ExternalPtrAddr>()(sexp.DangerousGetHandle());
            Marshal.Release(objPtr);
        }

        #endregion
    }
}
