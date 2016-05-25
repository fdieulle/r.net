using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RDotNet.AssemblyTest
{
    public class StaticClass
    {
        #region Test Same method

        public static void SameMethodName()
        {
            Console.WriteLine("SameMethodName() Call");
        }

        public static void SameMethodName(int intValue)
        {
            Console.WriteLine("SameMethodName(int) arg: {0}", intValue);
        }

        public static void SameMethodName(double doubleValue)
        {
            Console.WriteLine("SameMethodName(double) arg: {0}", doubleValue);
        }

        public static void SameMethodName(int[] intValue)
        {
            Console.WriteLine("SameMethodName(int[]) arg: {0}", string.Join(";", intValue));
        }

        public static void SameMethodName(double[] doubleValue)
        {
            Console.WriteLine("SameMethodName(double[]) arg: {0}", string.Join(";", doubleValue));
        }

        public static void SameMethodName(double doubleValue, int intValue)
        {
            Console.WriteLine("SameMethodName(double, int) arg1: {0}, arg2: {1}", doubleValue, intValue);
        }

        public static void SameMethodName(double[] doubleValue, int[] intValue)
        {
            Console.WriteLine("SameMethodName(double[], int[]) arg1: {0}, arg2: {1}", string.Join(";", doubleValue), string.Join(";", intValue));
        }

        public static void SameMethodName(double[] doubleValue, int intValue)
        {
            Console.WriteLine("SameMethodName(double[], int) arg1: {0}, arg2: {1}", string.Join(";", doubleValue), intValue);
        }

        public static void SameMethodName(double doubleValue, int[] intValue)
        {
            Console.WriteLine("SameMethodName(double, int[]) arg1: {0}, arg2: {1}", doubleValue, string.Join(";", intValue));
        }

        #endregion 

        #region Native values

        public static int CallWithInteger(int value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), value);
            return value;
        }

        public static int[] CallWithIntegerVector(int[] value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), string.Join(",", value));
            return value;
        }
        
        public static int[,] CallWithIntegerMatrix(int[,] value)
        {
            var rows = value.GetLength(0);
            var columns = value.GetLength(1);

            Console.WriteLine("{0} arg: rows{1}, columns{2}", MethodBase.GetCurrentMethod(), rows, columns);

            var sb = new StringBuilder();
            for (var i = 0; i < rows; i++)
            {
                sb.Append("[");
                for (var j = 0; j < columns; j++)
                {
                    if (j != 0)
                        sb.Append(", ");
                    sb.Append(value[i, j]);
                }
                sb.Append("]");
                Console.WriteLine(sb.ToString());
                sb.Clear();
            }
                
            return value;
        }

        public static double CallWithNumeric(double value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), value);
            return value;
        }

        public static double[] CallWithNumericVector(double[] value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), string.Join(",", value));
            return value;
        }

        public static double[,] CallWithNumericMatrix(double[,] value)
        {
            var rows = value.GetLength(0);
            var columns = value.GetLength(1);

            Console.WriteLine("{0} arg: rows{1}, columns{2}", MethodBase.GetCurrentMethod(), rows, columns);

            var sb = new StringBuilder();
            for (var i = 0; i < rows; i++)
            {
                sb.Append("[");
                for (var j = 0; j < columns; j++)
                {
                    if (j != 0)
                        sb.Append(", ");
                    sb.Append(value[i, j]);
                }
                sb.Append("]");
                Console.WriteLine(sb.ToString());
                sb.Clear();
            }

            return value;
        }

        public static bool CallWithLogical(bool value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), value);
            return value;
        }

        public static bool[] CallWithLogicalVector(bool[] value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), string.Join(",", value));
            return value;
        }

        public static bool[,] CallWithLogicalMatrix(bool[,] value)
        {
            var rows = value.GetLength(0);
            var columns = value.GetLength(1);

            Console.WriteLine("{0} arg: rows{1}, columns{2}", MethodBase.GetCurrentMethod(), rows, columns);

            var sb = new StringBuilder();
            for (var i = 0; i < rows; i++)
            {
                sb.Append("[");
                for (var j = 0; j < columns; j++)
                {
                    if (j != 0)
                        sb.Append(", ");
                    sb.Append(value[i, j]);
                }
                sb.Append("]");
                Console.WriteLine(sb.ToString());
                sb.Clear();
            }

            return value;
        }

        public static string CallWithCharacter(string value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), value);
            return value;
        }

        public static string[] CallWithCharacterVector(string[] value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), string.Join(",", value));
            return value;
        }

        public static string[,] CallWithCharacterMatrix(string[,] value)
        {
            var rows = value.GetLength(0);
            var columns = value.GetLength(1);

            Console.WriteLine("{0} arg: rows{1}, columns{2}", MethodBase.GetCurrentMethod(), rows, columns);

            var sb = new StringBuilder();
            for (var i = 0; i < rows; i++)
            {
                sb.Append("[");
                for (var j = 0; j < columns; j++)
                {
                    if (j != 0)
                        sb.Append(", ");
                    sb.Append(value[i, j]);
                }
                sb.Append("]");
                Console.WriteLine(sb.ToString());
                sb.Clear();
            }

            return value;
        }

        #endregion

        #region DateTime

        public static DateTime CallWithPOSIXctOrPOSIXltOrDate(DateTime value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), value);
            return value;
        }

        public static DateTime[] CallWithPOSIXctOrPOSIXltOrDateVector(DateTime[] value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), string.Join(",", value));
            return value;
        }

        public static DateTime[,] CallWithPOSIXctOrPOSIXltOrDateMatrix(DateTime[,] value)
        {
            var rows = value.GetLength(0);
            var columns = value.GetLength(1);

            Console.WriteLine("{0} arg: rows{1}, columns{2}", MethodBase.GetCurrentMethod(), rows, columns);

            var sb = new StringBuilder();
            for (var i = 0; i < rows; i++)
            {
                sb.Append("[");
                for (var j = 0; j < columns; j++)
                {
                    if (j != 0)
                        sb.Append(", ");
                    sb.Append(value[i, j]);
                }
                sb.Append("]");
                Console.WriteLine(sb.ToString());
                sb.Clear();
            }

            return value;
        }

        public static DateTime GetUtcNow()
        {
            Console.WriteLine("{0} {1}", MethodBase.GetCurrentMethod(), DateTime.UtcNow);
            return DateTime.UtcNow;
        }

        public static DateTime GetNow()
        {
            Console.WriteLine("{0} {1}", MethodBase.GetCurrentMethod(), DateTime.Now);
            return DateTime.Now;
        }

        #endregion

        #region TimeSpan

        public static TimeSpan CallWithDifftime(TimeSpan value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), value);
            return value;
        }

        public static TimeSpan[] CallWithDifftimeVector(TimeSpan[] value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), string.Join(",", value));
            return value;
        }

        public static TimeSpan[,] CallWithDifftimeMatrix(TimeSpan[,] value)
        {
            var rows = value.GetLength(0);
            var columns = value.GetLength(1);

            Console.WriteLine("{0} arg: rows{1}, columns{2}", MethodBase.GetCurrentMethod(), rows, columns);

            var sb = new StringBuilder();
            for (var i = 0; i < rows; i++)
            {
                sb.Append("[");
                for (var j = 0; j < columns; j++)
                {
                    if (j != 0)
                        sb.Append(", ");
                    sb.Append(value[i, j]);
                }
                sb.Append("]");
                Console.WriteLine(sb.ToString());
                sb.Clear();
            }

            return value;
        }

        #endregion

        #region List

        #region Array

        public static OneCtorData[] CallWithListToTypedArray(OneCtorData[] array)
        {
            var length = array != null ? array.Length : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), length);

            for (var i = 0; i < length; i++)
                Console.WriteLine("[{0}] {1}", i, array[i]);

            return array;
        }

        public static Array CallWithListToArray(Array array)
        {
            var length = array != null ? array.Length : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), length);

            for (var i = 0; i < length; i++)
                Console.WriteLine("[{0}] {1}", i, array.GetValue(i));

            return array;
        }

        #endregion // Array

        #region List

        public static List<OneCtorData> CallWithListToTypedList(List<OneCtorData> array)
        {
            var length = array != null ? array.Count : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), length);

            for (var i = 0; i < length; i++)
                Console.WriteLine("[{0}] {1}", i, array[i]);

            return array;
        }

        public static IList<OneCtorData> CallWithListToTypedIList(IList<OneCtorData> array)
        {
            var length = array != null ? array.Count : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), length);

            for (var i = 0; i < length; i++)
                Console.WriteLine("[{0}] {1}", i, array[i]);

            return array;
        }

        public static ICollection<OneCtorData> CallWithListToTypedICollection(ICollection<OneCtorData> array)
        {
            var length = array != null ? array.Count : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), length);

            var i = 0;
            foreach (var item in array)
            {
                Console.WriteLine("[{0}] {1}", i, item);
                i++;
            }

            return array;
        }

        public static IEnumerable<OneCtorData> CallWithListToTypedIEnumerable(IEnumerable<OneCtorData> array)
        {
            var length = array != null ? array.Count() : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), length);

            var i = 0;
            foreach (var item in array)
            {
                Console.WriteLine("[{0}] {1}", i, item);
                i++;
            }

            return array;
        }

        public static IList CallWithListToIList(IList array)
        {
            var length = array != null ? array.Count : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), length);

            for (var i = 0; i < length; i++)
                Console.WriteLine("[{0}] {1}", i, array[i]);

            return array;
        }

        public static ICollection CallWithListToICollection(ICollection array)
        {
            var length = array != null ? array.Count : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), length);

            var i = 0;
            foreach (var item in array)
            {
                Console.WriteLine("[{0}] {1}", i, item);
                i++;
            }

            return array;
        }

        public static IEnumerable CallWithListToIEnumerable(IEnumerable array)
        {
            //var length = array != null ? array.Count() : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), "??");

            var i = 0;
            foreach (var item in array)
            {
                Console.WriteLine("[{0}] {1}", i, item);
                i++;
            }

            return array;
        }

        public static IReadOnlyList<OneCtorData> CallWithListToTypedIReadOnlyList(IReadOnlyList<OneCtorData> array)
        {
            var length = array != null ? array.Count : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), length);

            var i = 0;
            foreach (var item in array)
            {
                Console.WriteLine("[{0}] {1}", i, item);
                i++;
            }

            return array;
        }

        public static IReadOnlyCollection<OneCtorData> CallWithListToTypedIReadOnlyCollection(IReadOnlyCollection<OneCtorData> array)
        {
            var length = array != null ? array.Count : 0;
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), length);

            var i = 0;
            foreach (var item in array)
            {
                Console.WriteLine("[{0}] {1}", i, item);
                i++;
            }

            return array;
        }

        #endregion // List

        #region Dictionary

        public static Dictionary<string, OneCtorData> CallWithListToDictionary(Dictionary<string, OneCtorData> dico)
        {
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), dico.Count);
            
            foreach (var pair in dico)
                Console.WriteLine("[{0}] {1}", pair.Key, pair.Value);
            
            return dico;
        }

        public static IDictionary<string, OneCtorData> CallWithListToIDictionary(IDictionary<string, OneCtorData> dico)
        {
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), dico.Count);

            foreach (var pair in dico)
                Console.WriteLine("[{0}] {1}", pair.Key, pair.Value);

            return dico;
        }

        public static ICollection<KeyValuePair<string, OneCtorData>> CallWithListToTypedICollection(ICollection<KeyValuePair<string, OneCtorData>> dico)
        {
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), dico.Count);

            foreach (var pair in dico)
                Console.WriteLine("[{0}] {1}", pair.Key, pair.Value);

            return dico;
        }

        public static IEnumerable<KeyValuePair<string, OneCtorData>> CallWithListToTypedIEnumerable(IEnumerable<KeyValuePair<string, OneCtorData>> dico)
        {
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), dico.Count());

            foreach (var pair in dico)
                Console.WriteLine("[{0}] {1}", pair.Key, pair.Value);

            return dico;
        }

        public static ICollection CallWithListToDicoICollection(ICollection dico)
        {
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), dico.Count);

            foreach (var pair in dico.OfType<KeyValuePair<string, OneCtorData>>())
                Console.WriteLine("[{0}] {1}", pair.Key, pair.Value);

            return dico;
        }

        public static IEnumerable CallWithListToDicoIEnumerable(IEnumerable dico)
        {
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), "??");

            foreach (var pair in dico.OfType<KeyValuePair<string, OneCtorData>>())
                Console.WriteLine("[{0}] {1}", pair.Key, pair.Value);

            return dico;
        }

        public static IReadOnlyDictionary<string, OneCtorData> CallWithListToTypedIReadOnlyDictionary(IReadOnlyDictionary<string, OneCtorData> dico)
        {
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), dico.Count);

            foreach (var pair in dico)
                Console.WriteLine("[{0}] {1}", pair.Key, pair.Value);

            return dico;
        }

        public static IReadOnlyCollection<KeyValuePair<string, OneCtorData>> CallWithListToTypedIReadOnlyCollection(IReadOnlyCollection<KeyValuePair<string, OneCtorData>> dico)
        {
            Console.WriteLine("{0} Length: {1}", MethodBase.GetCurrentMethod(), dico.Count);

            foreach (var pair in dico)
                Console.WriteLine("[{0}] {1}", pair.Key, pair.Value);

            return dico;
        }

        #endregion

        public static List<List<List<OneCtorData>>> CallWithListToTypedListRecurse(List<List<List<OneCtorData>>> array)
        {
            Console.WriteLine("{0}", MethodBase.GetCurrentMethod());

            var c1 = array.Count;
            Console.WriteLine("length: {0}", c1);
            for (var i = 0; i < c1; i++)
            {
                var c2 = array[i].Count;
                Console.WriteLine("[{0}] length: {1}", i, c2);
                for (var j = 0; j < c2; j++)
                {
                    var c3 = array[i][j].Count;
                    Console.WriteLine("[{0}][{1}] length: {2}", i, j, c3);
                    for (var k = 0; k < c3; k++)
                    {
                        Console.WriteLine("[{0}][{1}][{2}] {3}", i, j, k, array[i][j][k]);
                    }
                }
            }

            return array;
        }

        #endregion

        #region Enum

        public static EnumSample CallWithEnum(EnumSample value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), value);
            return value;
        }

        public static EnumSample[] CallWithEnumVector(EnumSample[] value)
        {
            Console.WriteLine("{0} arg: {1}", MethodBase.GetCurrentMethod(), string.Join(",", value));
            return value;
        }

        #endregion

        #region Custom methods

        public static void OptionalArguments(string[] arg1, int[] arg2 = null, bool arg3 = true)
        {
            Console.WriteLine("{0} arg1: {1}, arg2: {2}, arg3: {3}", MethodBase.GetCurrentMethod(), arg1, arg2, arg3);
        }

        public static void NullValueFromR(string[] arg1, object arg2, bool arg3)
        {
            Console.WriteLine("{0} arg1: {1}, arg2: {2}, arg3: {3}", MethodBase.GetCurrentMethod(), arg1, arg2, arg3);
        }

        #endregion
    }
}
