using System;
using System.Collections.Generic;
using RDotNet.ClrProxy.Loggers;
using RDotNet.Internals;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class RDotNetDataConverter : IDataConverter
    {
        #region Singleton

        private static readonly ILogger logger = Logger.Instance;

        private static readonly RDotNetDataConverter instance = new RDotNetDataConverter();
        public static RDotNetDataConverter Instance { get { return instance; } }

        private static readonly REngine engine;
        public static REngine Engine { get { return engine; } }

        static RDotNetDataConverter()
        {
            engine = REngine.GetInstance(initialize: false);
            engine.Initialize(setupMainLoop: false);
            engine.AutoPrint = false;
        }

        #endregion

        private readonly Dictionary<SymbolicExpressionType, Func<SymbolicExpression, IConverter>> converterFactories = new Dictionary<SymbolicExpressionType, Func<SymbolicExpression, IConverter>>();
        private readonly Dictionary<Type, Func<object, SymbolicExpression>> convertersBack = new Dictionary<Type, Func<object, SymbolicExpression>>();

        private RDotNetDataConverter()
        {
            SetupFromRConverters();
            SetupToRConverters();
        }

        #region Implementation of IDataConverter

        public IConverter GetConverter(long address)
        {
            var sexp = engine.CreateFromNativeSexp(new IntPtr(address));

            logger.DebugFormat("SEXP type: {0}", sexp.Type);

            Func<SymbolicExpression, IConverter> factory;
            if (converterFactories.TryGetValue(sexp.Type, out factory))
                return factory(sexp);

            throw new InvalidCastException(string.Format("Unable to find a converter from R type: {0}", sexp.Type));
        }

        public object ConvertBack(Type type, object data)
        {
            var sexp = ConvertToSexp(type, data);
            if (sexp == null)
                return engine.NilValue.DangerousGetHandle();

            return sexp.DangerousGetHandle();
        }

        public SymbolicExpression ConvertToSexp(Type type, object data)
        {
            if (data == null)
                return engine.NilValue;

            Func<object, SymbolicExpression> convert;
            if (convertersBack.TryGetValue(type, out convert))
            {
                var sexp = convert(data);
                if (sexp == null)
                    return engine.NilValue;

                return sexp;
            }

            if (data.GetType().IsEnum)
                return ConvertToSexp(typeof (string), data.ToString());

            // Try to convert a generic list or dictionary first
            SymbolicExpression result;
            if (SexpListConverter.TryConvertBack(engine, this, data, out result))
                return result;

            // Otherwise convert to an external pointer
            return SexpExternalPtrConverter.ConvertBack(engine, data);
        }

        #endregion

        #region Setup from R converters

        private void SetupFromRConverters()
        {
            SetupFromRConverter(SymbolicExpressionType.Null, p => NullConverter.Instance);
            SetupFromRConverter(SymbolicExpressionType.CharacterVector, ConvertFromCharacterVector);
            SetupFromRConverter(SymbolicExpressionType.IntegerVector, ConvertFromIntegerVector);
            SetupFromRConverter(SymbolicExpressionType.LogicalVector, ConvertFromLogicalVector);
            SetupFromRConverter(SymbolicExpressionType.NumericVector, ConvertFromNumericalVector);
            SetupFromRConverter(SymbolicExpressionType.ExternalPointer, ConvertFromExternalPointer);
            SetupFromRConverter(SymbolicExpressionType.List, ConvertFromList);
        }

        public void SetupFromRConverter(
            SymbolicExpressionType type,
            Func<SymbolicExpression, IConverter> createConverter)
        {
            if (logger.IsDebugEnabled)
            {
                logger.DebugFormat(
                    converterFactories.ContainsKey(type)
                        ? "Override converter R -> C#, Type: {0}"
                        : "Setup converter R -> C#, Type: {0}", type);
            }
            converterFactories[type] = createConverter;
        }

        public bool RemoveFromRConverter(SymbolicExpressionType type)
        {
            if (logger.IsDebugEnabled && converterFactories.ContainsKey(type))
                logger.DebugFormat("Remove converter R -> C#, Type: {0}", type);

            return converterFactories.Remove(type);
        }

        public Func<SymbolicExpression, IConverter> GetFromRConverter(SymbolicExpressionType type)
        {
            Func<SymbolicExpression, IConverter> converter;
            converterFactories.TryGetValue(type, out converter);
            return converter;
        }

        protected virtual IConverter ConvertFromCharacterVector(SymbolicExpression sexp)
        {
            if (sexp.IsMatrix())
                return new SexpMatrixConverter<string>(sexp.AsCharacterMatrix());

            return new SexpCharacterVectorConverter(sexp.AsCharacter());
        }

        protected virtual IConverter ConvertFromIntegerVector(SymbolicExpression sexp)
        {
            var isDifftime = sexp.IsDifftime();

            if (sexp.IsMatrix())
            {
                if (isDifftime)
                    return new SexpIntegerDifftimeMatrixConverter(sexp.AsIntegerMatrix());

                return new SexpMatrixConverter<int>(sexp.AsIntegerMatrix());
            }

            if (isDifftime)
                return new SexpIntegerDifftimeVectorConverter(sexp.AsInteger());

            return new SexpVectorConverter<int>(sexp.AsInteger());
        }

        protected virtual IConverter ConvertFromLogicalVector(SymbolicExpression sexp)
        {
            if (sexp.IsMatrix())
                return new SexpMatrixConverter<bool>(sexp.AsLogicalMatrix());

            return new SexpVectorConverter<bool>(sexp.AsLogical());
        }

        protected virtual IConverter ConvertFromNumericalVector(SymbolicExpression sexp)
        {
            var isPosixct = sexp.IsPosixct();
            var isDifftime = !isPosixct && sexp.IsDifftime();

            if (sexp.IsMatrix())
            {
                if (isPosixct)
                    return new SexpPosixctMatrixConverter(sexp.AsNumericMatrix());
                if(isDifftime)
                    return new SexpNumericDifftimeMatrixConverter(sexp.AsNumericMatrix());

                return new SexpMatrixConverter<double>(sexp.AsNumericMatrix());
            }

            if (isPosixct)
                return new SexpPosixctVectorConverter(sexp.AsNumeric());
            if (isDifftime)
                return new SexpNumericDifftimeVectorConverter(sexp.AsNumeric());

            return new SexpVectorConverter<double>(sexp.AsNumeric());
        }

        protected virtual IConverter ConvertFromExternalPointer(SymbolicExpression sexp)
        {
            return new SexpExternalPtrConverter(sexp);
        }

        protected virtual IConverter ConvertFromList(SymbolicExpression sexp)
        {
            return new SexpListConverter(sexp.AsList(), this);
        }

        #endregion

        #region Setup To R Converters

        private void SetupToRConverters()
        {
            SetupToRConverter(typeof(void), p => null);
            SetupToRConverter(typeof(string), p => engine.CreateCharacter((string)p));
            SetupToRConverter(typeof(string[]), p => engine.CreateCharacterVector((string[])p));
            SetupToRConverter(typeof(List<string>), p => engine.CreateCharacterVector((IEnumerable<string>)p));
            SetupToRConverter(typeof(IList<string>), p => engine.CreateCharacterVector((IEnumerable<string>)p));
            SetupToRConverter(typeof(ICollection<string>), p => engine.CreateCharacterVector((IEnumerable<string>)p));
            SetupToRConverter(typeof(IEnumerable<string>), p => engine.CreateCharacterVector((IEnumerable<string>)p));
            SetupToRConverter(typeof(string[,]), p => engine.CreateCharacterMatrix((string[,])p));
            
            SetupToRConverter(typeof(int), p => engine.CreateInteger((int)p));
            SetupToRConverter(typeof(int[]), p => engine.CreateIntegerVector((int[])p));
            SetupToRConverter(typeof(List<int>), p => engine.CreateIntegerVector((IEnumerable<int>)p));
            SetupToRConverter(typeof(IList<int>), p => engine.CreateIntegerVector((IEnumerable<int>)p));
            SetupToRConverter(typeof(ICollection<int>), p => engine.CreateIntegerVector((IEnumerable<int>)p));
            SetupToRConverter(typeof(IEnumerable<int>), p => engine.CreateIntegerVector((IEnumerable<int>)p));
            SetupToRConverter(typeof(int[,]), p => engine.CreateIntegerMatrix((int[,])p));
            
            SetupToRConverter(typeof(bool), p => engine.CreateLogical((bool)p));
            SetupToRConverter(typeof(bool[]), p => engine.CreateLogicalVector((bool[])p));
            SetupToRConverter(typeof(List<bool>), p => engine.CreateLogicalVector((IEnumerable<bool>)p));
            SetupToRConverter(typeof(IList<bool>), p => engine.CreateLogicalVector((IEnumerable<bool>)p));
            SetupToRConverter(typeof(ICollection<bool>), p => engine.CreateLogicalVector((IEnumerable<bool>)p));
            SetupToRConverter(typeof(IEnumerable<bool>), p => engine.CreateLogicalVector((IEnumerable<bool>)p));
            SetupToRConverter(typeof(bool[,]), p => engine.CreateLogicalMatrix((bool[,])p));
            
            SetupToRConverter(typeof(double), p => engine.CreateNumeric((double)p));
            SetupToRConverter(typeof(double[]), p => engine.CreateNumericVector((double[])p));
            SetupToRConverter(typeof(List<double>), p => engine.CreateNumericVector((IEnumerable<double>)p));
            SetupToRConverter(typeof(IList<double>), p => engine.CreateNumericVector((IEnumerable<double>)p));
            SetupToRConverter(typeof(ICollection<double>), p => engine.CreateNumericVector((IEnumerable<double>)p));
            SetupToRConverter(typeof(IEnumerable<double>), p => engine.CreateNumericVector((IEnumerable<double>)p));
            SetupToRConverter(typeof(double[,]), p => engine.CreateNumericMatrix((double[,])p));
            
            SetupToRConverter(typeof(DateTime), p => engine.CreatePosixct((DateTime)p));
            SetupToRConverter(typeof(DateTime[]), p => engine.CreatePosixctVector((DateTime[])p));
            SetupToRConverter(typeof(List<DateTime>), p => engine.CreatePosixctVector((IEnumerable<DateTime>)p));
            SetupToRConverter(typeof(IList<DateTime>), p => engine.CreatePosixctVector((IEnumerable<DateTime>)p));
            SetupToRConverter(typeof(ICollection<DateTime>), p => engine.CreatePosixctVector((IEnumerable<DateTime>)p));
            SetupToRConverter(typeof(IEnumerable<DateTime>), p => engine.CreatePosixctVector((IEnumerable<DateTime>)p));
            SetupToRConverter(typeof(DateTime[,]), p => engine.CreatePosixctMatrix((DateTime[,])p));
            
            SetupToRConverter(typeof(TimeSpan), p => engine.CreateDifftime((TimeSpan)p));
            SetupToRConverter(typeof(TimeSpan[]), p => engine.CreateDifftimeVector((TimeSpan[])p));
            SetupToRConverter(typeof(List<TimeSpan>), p => engine.CreateDifftimeVector((IEnumerable<TimeSpan>)p));
            SetupToRConverter(typeof(IList<TimeSpan>), p => engine.CreateDifftimeVector((IEnumerable<TimeSpan>)p));
            SetupToRConverter(typeof(ICollection<TimeSpan>), p => engine.CreateDifftimeVector((IEnumerable<TimeSpan>)p));
            SetupToRConverter(typeof(IEnumerable<TimeSpan>), p => engine.CreateDifftimeVector((IEnumerable<TimeSpan>)p));
            SetupToRConverter(typeof(TimeSpan[,]), p => engine.CreateDifftimeMatrix((TimeSpan[,])p));
        }

        public void SetupToRConverter(Type type, Func<object, SymbolicExpression> converter)
        {
            if (type == null) return;

            if (logger.IsDebugEnabled)
            {
                logger.DebugFormat(
                    convertersBack.ContainsKey(type)
                        ? "Override converter C# -> R, Type: {0}"
                        : "Setup converter C# -> R, Type: {0}", type);
            }

            convertersBack[type] = converter;
        }

        public bool RemoveToRConverter(Type type)
        {
            if (type == null) return false;

            if (logger.IsDebugEnabled && convertersBack.ContainsKey(type))
                logger.DebugFormat("Remove converter C# -> R, Type: {0}", type);

            return convertersBack.Remove(type);
        }

        public Func<object, SymbolicExpression> GetToRConverter(Type type)
        {
            if (type == null) return null;

            Func<object, SymbolicExpression> converter;
            convertersBack.TryGetValue(type, out converter);
            return converter;
        }

        #endregion
    }
}
