using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RDotNet.ClrProxy.Converters.RDotNet
{
    public class SexpListConverter : IConverter
    {
        private static readonly Type[] keyTypes = new[] { typeof(string) };
        private static readonly Type[] defaultItemType = new[] { typeof(object) };
        private static readonly HashSet<Type> listTypeDefinitions;
        private static readonly HashSet<Type> dictionaryTypeDefinitions;

        static SexpListConverter()
        {
            var list = typeof(List<object>)
                .GetFullHierarchy()
                .Where(p => p.IsGenericType)
                .Select(p => p.GetGenericTypeDefinition())
                .ToArray();

            var dictionary = typeof(Dictionary<string, object>).GetFullHierarchy()
                .Where(p => p.IsGenericType)
                .Select(p => p.GetGenericTypeDefinition())
                .ToArray();

            listTypeDefinitions = new HashSet<Type>(list);
            dictionaryTypeDefinitions = new HashSet<Type>(dictionary);

            var intersect = dictionary.Intersect(list);
            foreach (var type in intersect)
                dictionaryTypeDefinitions.Remove(type);
        }

        private readonly int length;
        private readonly IConverter[] converters;
        private readonly string[] names;
        private readonly Type[] types;
        private readonly Type[] intersectedItemType;

        public SexpListConverter(GenericVector sexp, IDataConverter converter)
        {
            var array = sexp.ToArray();
            length = sexp.Length;

            intersectedItemType = null;
            converters = new IConverter[length];
            for (var i = 0; i < length; i++)
            {
                converters[i] = converter.GetConverter(array[i].DangerousGetHandle().ToInt64());
                if (converters[i] == null)
                    throw new InvalidDataException("Unable to get convert for data at index: " + i + " in List");

                var itemTypes = converters[i].GetClrTypes();
                intersectedItemType = intersectedItemType == null
                    ? itemTypes
                    : intersectedItemType.Intersect(itemTypes);
            }

            if (intersectedItemType == null)
                intersectedItemType = new[] { typeof(object) };

            var fullTypes = new List<Type>();
            names = sexp.Names;
            if (names != null)
            {
                fullTypes.AddRange(keyTypes.GetDictionaryTypes(intersectedItemType));
                if (names.Length != length)
                {
                    var swap = new string[length];
                    for (var i = 0; i < length; i++)
                    {
                        swap[i] = i < names.Length
                            ? names[i]
                            : "Column " + (i + 1);
                    }
                    names = swap;
                }
            }

            fullTypes.AddRange(intersectedItemType.GetListOrArrayTypes());

            var count = fullTypes.Count;
            types = fullTypes[0].GetFullHierarchy();
            for (var i = 1; i < count; i++)
                types = types.Union(fullTypes[i].GetFullHierarchy());
        }

        #region Implementation of IConverter

        public Type[] GetClrTypes()
        {
            return types;
        }

        public object Convert(Type type)
        {
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                if (listTypeDefinitions.Contains(genericTypeDefinition))
                {
                    var genericType = type.GetGenericArguments()[0];
                    var list = (IList)Activator.CreateInstance(type.IsInterface
                        ? typeof(List<>).MakeGenericType(genericType)
                        : type);
                    for (var i = 0; i < length; i++)
                        list.Add(converters[i].Convert(genericType));
                    return list;
                }

                if (dictionaryTypeDefinitions.Contains(genericTypeDefinition))
                {
                    var genericType = type.GetGenericArguments()[1];
                    var dico = (IDictionary)Activator.CreateInstance(type.IsInterface
                        ? typeof(Dictionary<,>).MakeGenericType(typeof(string), genericType)
                        : type);
                    for (var i = 0; i < length; i++)
                        dico.Add(names[i], converters[i].Convert(genericType));
                    return dico;
                }
            }

            if (type.IsArray || type == typeof(Array))
            {
                var genericType = type.GetElementType() ?? intersectedItemType[0];
                var array = Array.CreateInstance(genericType, length);
                for (var i = 0; i < length; i++)
                    array.SetValue(converters[i].Convert(genericType), i);
                return array;
            }

            var defaultList = new List<object>(length);
            for (var i = 0; i < length; i++)
            {
                var itemTypes = converters[i].GetClrTypes() ?? defaultItemType;
                defaultList.Add(converters[i].Convert(itemTypes.Length == 0 ? defaultItemType[0] : itemTypes[0]));
            }
            return defaultList;
        }

        #endregion

        public static bool TryConvertBack(REngine engine, RDotNetDataConverter dataConverter, object data, out SymbolicExpression result)
        {
            var type = data.GetType();
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                if (dictionaryTypeDefinitions.Contains(genericTypeDefinition))
                {
                    var genericArguments = type.GetGenericArguments();
                    if (genericArguments[0] == typeof(string))
                    {
                        var sexp = dicoToSexpMethod.MakeGenericMethod(new[] {genericArguments[1]})
                             .Invoke(null, new[] {engine, dataConverter, data}) as SymbolicExpression;
                        if (sexp != null)
                        {
                            result = sexp;
                            return true;
                        }
                    }
                }

                if (listTypeDefinitions.Contains(genericTypeDefinition))
                {
                    var enumerable = data as IEnumerable;
                    if (enumerable != null)
                    {
                        result = new GenericVector(engine, ListToSexp(engine, dataConverter, enumerable));
                        return true;
                    }
                }
            }

            if (type.IsArray)
            {
                var array = data as IEnumerable;
                if (array != null)
                {
                    result = new GenericVector(engine, ListToSexp(engine, dataConverter, array));
                    return true;
                }
            }

            result = engine.NilValue;
            return false;
        }

// ReSharper disable UnusedMember.Local
        private static readonly MethodInfo dicoToSexpMethod = MethodBase.GetCurrentMethod().DeclaringType
            .GetMethod("DicoToSexp", BindingFlags.NonPublic | BindingFlags.Static);
        private static SymbolicExpression DicoToSexp<T>(REngine engine, RDotNetDataConverter dataConverter, IEnumerable<KeyValuePair<string, T>> enumerable)
// ReSharper restore UnusedMember.Local
        {
            var type = typeof (T);
            var array = enumerable.ToArray();
            var length = array.Length;

            var values = new SymbolicExpression[length];
            var names = new string[length];
            for (var i = 0; i < length; i++)
            {
                names[i] = array[i].Key;
                var sexp = dataConverter.ConvertToSexp(type, array[i].Value);
                values[i] = sexp ?? engine.NilValue;
            }

            var vector = new GenericVector(engine, values);
            vector.SetNames(names);
            return vector;
        }

        private static IEnumerable<SymbolicExpression> ListToSexp(REngine engine, RDotNetDataConverter dataConverter, IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                if (item == null)
                    yield return engine.NilValue;
                else
                {
                    var sexp = dataConverter.ConvertToSexp(item.GetType(), item);
                    if (sexp == null)
                        yield return engine.NilValue;
                    else yield return sexp;   
                }
            }
        }

    }
}
