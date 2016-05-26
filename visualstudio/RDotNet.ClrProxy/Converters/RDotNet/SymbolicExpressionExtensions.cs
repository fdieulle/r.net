using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using RDotNet.ClrProxy.Resources;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr R_MakeExternalPtr(IntPtr args, IntPtr tag, IntPtr prot);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr R_ExternalPtrTag(IntPtr args);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr R_ExternalPtrAddr(IntPtr args);

    public static class SymbolicExpressionExtensions
    {
        #region Faster than RDotNet extensions

        /// <summary>
        /// Creates a new NumericVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static SymbolicExpression CreateNumericVector(this REngine engine, double[] vector)
        {
            if (engine == null)
                throw new ArgumentNullException();
            
            if (!engine.IsRunning)
                throw new ArgumentException();

            return new NumericVector(engine, vector);
        }

        /// <summary>
        /// Creates a new IntegerVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static IntegerVector CreateIntegerVector(this REngine engine, int[] vector)
        {
            if (engine == null)
                throw new ArgumentNullException("engine");
            if (!engine.IsRunning)
                throw new ArgumentException("engine");
            
            return new IntegerVector(engine, vector);
        }

        #endregion

        #region DateTime

        public static bool IsPosixct(this SymbolicExpression sexp)
        {
            return sexp.GetAttributeNames().Any(p => string.Equals("class", p))
                && sexp.GetAttribute("class").AsCharacter().ToArray().Any(p => string.Equals("POSIXct", p));
        }

        public static bool IsPosixlt(this SymbolicExpression sexp)
        {
            return sexp.GetAttributeNames().Any(p => string.Equals("class", p))
                && sexp.GetAttribute("class").AsCharacter().ToArray().Any(p => string.Equals("POSIXlt", p));
        }

        public static TimeZoneInfo GetWindowsTimezone(this SymbolicExpression sexp)
        {
            if (sexp.GetAttributeNames().Any(p => string.Equals("tzone", p)))
            {
                var tzone = sexp.GetAttribute("tzone").AsCharacter().ToArray().FirstOrDefault();
                return tzone.GetWindowsTimezone();
            }

            return TimeZoneInfo.Local;
        }

        public static SymbolicExpression CreatePosixct(this REngine engine, DateTime value)
        {
            return engine.CreatePosixctVector(new [] {value});
        }

        public static SymbolicExpression CreatePosixctVector(this REngine engine, DateTime[] data)
        {
            string[] tzone;
            var numeric = data.ToTicks(out tzone);
            var sexp = engine.CreateNumericVector(numeric);
            return sexp.AddPosixctAttributes(tzone);
        }

        public static SymbolicExpression CreatePosixctVector(this REngine engine, IEnumerable<DateTime> data)
        {
            string[] tzone;
            var numeric = data.ToArray().ToTicks(out tzone);
            var sexp = engine.CreateNumericVector(numeric);
            return sexp.AddPosixctAttributes(tzone);
        }

        public static SymbolicExpression CreatePosixctMatrix(this REngine engine, DateTime[,] data)
        {
            string[] tzone;
            var numeric = data.ToTicks(out tzone);
            var sexp = engine.CreateNumericMatrix(numeric);
            return sexp.AddPosixctAttributes(tzone);
        }

        public static SymbolicExpression AddPosixctAttributes(this SymbolicExpression sexp, IEnumerable<string> tzone)
        {
            sexp.SetAttribute("class", sexp.Engine.CreateCharacterVector(new[] { "POSIXct", "POSIXt" }));
            sexp.SetAttribute("tzone", sexp.Engine.CreateCharacterVector(tzone));
            return sexp;
        }

        #endregion

        #region TimeSpan

        public static bool IsDifftime(this SymbolicExpression sexp)
        {
            return sexp.GetAttributeNames().Any(p => string.Equals("class", p))
                && sexp.GetAttribute("class").AsCharacter().ToArray().Any(p => string.Equals("difftime", p));
        }

        private const string SECS = "secs";
        private const string MINS = "mins";
        private const string HOURS = "hours";
        private const string DAYS = "days";
        private const string WEEKS = "weeks";

        public static string GetUnits(this SymbolicExpression sexp)
        {
            string units = null;
            if (sexp.GetAttributeNames().Any(p => string.Equals("units", p)))
                units = sexp.GetAttribute("units").AsCharacter().FirstOrDefault();
            
            switch (units)
            {
                case WEEKS:
                    return WEEKS;
                case DAYS:
                    return DAYS;
                case HOURS:
                    return HOURS;
                case MINS:
                    return MINS;
                default:
                    return SECS;
            }
        }

        public static SymbolicExpression CreateDifftime(this REngine engine, TimeSpan data)
        {
            return engine.CreateDifftimeVector(new[] {data});
        }

        public static SymbolicExpression CreateDifftimeVector(this REngine engine, TimeSpan[] data)
        {
            var numeric = data.FromTimeSpan();
            var sexp = engine.CreateNumericVector(numeric);
            return sexp.AddDifftimeAttributes();
        }

        public static SymbolicExpression CreateDifftimeVector(this REngine engine, IEnumerable<TimeSpan> data)
        {
            var numeric = data.ToArray().FromTimeSpan();
            var sexp = engine.CreateNumericVector(numeric);
            return sexp.AddDifftimeAttributes();
        }

        public static SymbolicExpression CreateDifftimeMatrix(this REngine engine, TimeSpan[,] data)
        {
            var numeric = data.FromTimeSpan();
            var sexp = engine.CreateNumericMatrix(numeric);
            return sexp.AddDifftimeAttributes();
        }

        public static SymbolicExpression AddDifftimeAttributes(this SymbolicExpression sexp, string units = SECS)
        {
            sexp.SetAttribute("class", sexp.Engine.CreateCharacterVector(new[] { "difftime" }));
            sexp.SetAttribute("units", sexp.Engine.CreateCharacterVector(new[] { units }));
            return sexp;
        }

        public static TimeSpan[] ToTimespan(this double[] values, string units)
        {
            var length = values.Length;
            var result = new TimeSpan[length];
            switch (units)
            {
                case WEEKS:
                    for (var i = 0; i < length; i++)
                        result[i] = TimeSpan.FromDays(7 * values[i]);
                    return result;
                case DAYS:
                    for (var i = 0; i < length; i++)
                        result[i] = TimeSpan.FromDays(values[i]);
                    return result;
                case HOURS:
                    for (var i = 0; i < length; i++)
                        result[i] = TimeSpan.FromHours(values[i]);
                    return result;
                case MINS:
                    for (var i = 0; i < length; i++)
                        result[i] = TimeSpan.FromMinutes(values[i]);
                    return result;
                default:
                    for (var i = 0; i < length; i++)
                        result[i] = TimeSpan.FromSeconds(values[i]);
                    return result;
            }
        }

        public static TimeSpan[,] ToTimespan(this double[,] values, string units)
        {
            var nrow = values.GetLength(0);
            var ncol = values.GetLength(1);

            var result = new TimeSpan[nrow, ncol];
            switch (units)
            {
                case WEEKS:
                    for (var i = 0; i < nrow; i++)
                        for (var j = 0; j < ncol; j++)
                            result[i, j] = TimeSpan.FromDays(7 * values[i, j]);
                    return result;
                case DAYS:
                    for (var i = 0; i < nrow; i++)
                        for (var j = 0; j < ncol; j++)
                            result[i, j] = TimeSpan.FromDays(values[i, j]);
                    return result;
                case HOURS:
                    for (var i = 0; i < nrow; i++)
                        for (var j = 0; j < ncol; j++)
                            result[i, j] = TimeSpan.FromHours(values[i, j]);
                    return result;
                case MINS:
                    for (var i = 0; i < nrow; i++)
                        for (var j = 0; j < ncol; j++)
                            result[i, j] = TimeSpan.FromMinutes(values[i, j]);
                    return result;
                default:
                    for (var i = 0; i < nrow; i++)
                        for (var j = 0; j < ncol; j++)
                            result[i, j] = TimeSpan.FromSeconds(values[i, j]);
                    return result;
            }
        }

        public static TimeSpan[] ToTimespan(this int[] values, string units)
        {
            var length = values.Length;
            var result = new TimeSpan[length];
            switch (units)
            {
                case WEEKS:
                    for (var i = 0; i < length; i++)
                        result[i] = TimeSpan.FromDays(7 * values[i]);
                    return result;
                case DAYS:
                    for (var i = 0; i < length; i++)
                        result[i] = TimeSpan.FromDays(values[i]);
                    return result;
                case HOURS:
                    for (var i = 0; i < length; i++)
                        result[i] = TimeSpan.FromHours(values[i]);
                    return result;
                case MINS:
                    for (var i = 0; i < length; i++)
                        result[i] = TimeSpan.FromMinutes(values[i]);
                    return result;
                default:
                    for (var i = 0; i < length; i++)
                        result[i] = TimeSpan.FromSeconds(values[i]);
                    return result;
            }
        }

        public static TimeSpan[,] ToTimespan(this int[,] values, string units)
        {
            var nrow = values.GetLength(0);
            var ncol = values.GetLength(1);

            var result = new TimeSpan[nrow, ncol];
            switch (units)
            {
                case WEEKS:
                    for (var i = 0; i < nrow; i++)
                        for (var j = 0; j < ncol; j++)
                            result[i, j] = TimeSpan.FromDays(7 * values[i, j]);
                    return result;
                case DAYS:
                    for (var i = 0; i < nrow; i++)
                        for (var j = 0; j < ncol; j++)
                            result[i, j] = TimeSpan.FromDays(values[i, j]);
                    return result;
                case HOURS:
                    for (var i = 0; i < nrow; i++)
                        for (var j = 0; j < ncol; j++)
                            result[i, j] = TimeSpan.FromHours(values[i, j]);
                    return result;
                case MINS:
                    for (var i = 0; i < nrow; i++)
                        for (var j = 0; j < ncol; j++)
                            result[i, j] = TimeSpan.FromMinutes(values[i, j]);
                    return result;
                default:
                    for (var i = 0; i < nrow; i++)
                        for (var j = 0; j < ncol; j++)
                            result[i, j] = TimeSpan.FromSeconds(values[i, j]);
                    return result;
            }
        }

        public static double[] FromTimeSpan(this TimeSpan[] timespans)
        {
            var length = timespans.Length;
            var result = new double[length];
            for (var i = 0; i < length; i++)
                result[i] = timespans[i].TotalSeconds;
            return result;
        }

        public static double[,] FromTimeSpan(this TimeSpan[,] timespans)
        {
            var nrow = timespans.GetLength(0);
            var ncol = timespans.GetLength(1);

            var result = new double[nrow, ncol];
            for (var i = 0; i < nrow; i++)
                for (var j = 0; j < ncol; j++)
                    result[i, j] = timespans[i, j].TotalSeconds;
            return result;
        }

        #endregion

        #region Enums

        public static object ToEnum(this string[] values, Type type, bool single = false)
        {
            return toEnumArrayMethod.MakeGenericMethod(type).Invoke(null, new object[] { values, single });
        }

// ReSharper disable UnusedMember.Local
        private static readonly MethodInfo toEnumArrayMethod = MethodBase.GetCurrentMethod()
            .DeclaringType.GetMethod("ToEnumArray", BindingFlags.NonPublic | BindingFlags.Static);
        private static object ToEnumArray<TEnum>(this string[] values, bool single) where TEnum : struct
// ReSharper restore UnusedMember.Local
        {
            var length = values.Length;
            var array = new TEnum[length];
            for (var i = 0; i < length; i++)
            {
                TEnum value;
                if (Enum.TryParse(values[i], true, out value))
                    array[i] = value;
            }
            
            return single 
                ? (object) (length == 0 ? default(TEnum) : array[0])
                : array;
        }

        #endregion
    }
}
