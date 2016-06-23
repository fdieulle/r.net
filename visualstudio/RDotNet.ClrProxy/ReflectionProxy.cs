using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RDotNet.ClrProxy.Converters;
using RDotNet.ClrProxy.Loggers;

namespace RDotNet.ClrProxy
{
    public static class ReflectionProxy
    {
        private static readonly Logger logger = Logger.Instance;

        public static bool IsFullyQualifiedAssemblyName(this string assemblyName)
        {
            return assemblyName.Contains("PublicKeyToken=");
        }

        #region Looking for types

        private static readonly Type[] defaultTypeArray = new[] { typeof(object) };

        public static bool TryGetType(this string typeName, out Type type, out string errorMsg)
        {
            errorMsg = null;
            type = null;
            if (string.IsNullOrEmpty(typeName))
            {
                errorMsg = "Missing type name beause of null or empty";
                return false;
            }

            type = Type.GetType(typeName);
            if (type != null)
                return true;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var split = typeName.Split(',');
            if (split.Length > 1)
            {
                // Syntax: 'Namespace.Type, Assembly'

                var assemblyName = split[1].Trim();
                var assembly = assemblies.FirstOrDefault(p => string.Equals(p.GetName().Name, assemblyName));
                if (assembly == null)
                {
                    errorMsg = string.Format("Assembly not found {0}", assemblyName);
                    return false;
                }

                type = assembly.GetType(split[0].Trim());
                if (type != null)
                    return true;
            }

            // Syntax: 'Namespace.Type'
            typeName = split[0].Trim();
            var length = assemblies.Length;
            for (var i = 0; i < length; i++)
            {
                var types = assemblies[i].GetTypes();
                var l = types.Length;
                for (var j = 0; j < l; j++)
                {
                    if (!string.Equals(types[j].FullName, typeName))
                        continue;

                    type = types[j];
                    return true;
                }
            }

            errorMsg = string.Format("Type {0} not found.", typeName);
            return false;
        }

        public static Type[] GetFullHierarchy(this Type type)
        {
            if (type == null)
                return defaultTypeArray;

            var priorities = new Dictionary<Type, int>();
            var queue = new Queue<Type>();
            queue.Enqueue(type);
            var priority = 0;
            while (queue.Count != 0)
            {
                type = queue.Dequeue();

                if (!priorities.ContainsKey(type))
                    priorities.Add(type, priority++);

                var interfaces = type.GetInterfaces();
                var length = interfaces.Length;
                for (var i = 0; i < length; i++)
                {
                    if (!priorities.ContainsKey(interfaces[i]))
                        queue.Enqueue(interfaces[i]);

                    priorities[interfaces[i]] = priority++;
                }

                if (type.BaseType != null)
                    queue.Enqueue(type.BaseType);
            }

            if (!priorities.ContainsKey(typeof(object)))
                priorities.Add(typeof(object), int.MaxValue);

            return priorities.OrderBy(p => p.Value).Select(p => p.Key).ToArray();
        }

        public static Type[] Intersect(this Type[] x, Type[] y)
        {
            if (x == null || y == null)
                return null;

            var result = new List<Type>();

            var xl = x.Length;
            var yl = y.Length;

            for (var i = 0; i < xl; i++)
            {
                for (var j = 0; j < yl; j++)
                {
                    if (x[i] == y[j])
                        result.Add(x[i]);
                }
            }

            return result.ToArray();
        }

        public static Type[] Union(this Type[] x, Type[] y)
        {
            if (x == null)
                return y;
            if (y == null)
                return x;

            var result = new List<Type>();
            var unicity = new HashSet<Type>();
            var xl = x.Length;
            var yl = y.Length;
            int i = 0, j = 0;
            while (i < xl)
            {
                if (!unicity.Contains(x[i]))
                {
                    for (var k = j; k < yl; k++)
                    {
                        if (x[i] == y[k])
                        {
                            for (; j < k; j++)
                            {
                                if (!unicity.Contains(y[j]))
                                {
                                    result.Add(y[j]);
                                    unicity.Add(y[j]);
                                }
                            }
                            break;
                        }
                    }

                    result.Add(x[i]);
                    unicity.Add(x[i]);
                }

                i++;
            }

            for (; j < yl; j++)
            {
                if (!unicity.Contains(y[j]))
                {
                    result.Add(y[j]);
                    unicity.Add(y[j]);
                }
            }

            return result.ToArray();
        }

        public static List<Type> GetDictionaryTypes(this Type[] keyTypes, Type[] valueTypes)
        {
            if (keyTypes == null || valueTypes == null) return new List<Type>();

            var kl = keyTypes.Length;
            var vl = valueTypes.Length;
            var result = new List<Type>(kl * vl);
            for (var i = 0; i < kl; i++)
            {
                for (var j = 0; j < vl; j++)
                {
                    result.Add(typeof(Dictionary<,>)
                        .MakeGenericType(new[] { keyTypes[i], valueTypes[j] }));
                }
            }

            return result;
        }

        public static List<Type> GetListOrArrayTypes(this Type[] valueTypes)
        {
            if (valueTypes == null) return new List<Type>();

            var length = valueTypes.Length;
            var result = new List<Type>(length * 2);
            for (var i = 0; i < length; i++)
            {
                result.Add(valueTypes[i].MakeArrayType());
                result.Add(typeof(List<>).MakeGenericType(new[] { valueTypes[i] }));
            }
            result.Add(typeof(Array));

            return result;
        }

        public static Type[] GetGenericTypeDefinitions(this Type[] types)
        {
            return null;
        }

        #endregion

        public static bool TryGetMethod(this Type type,
            string methodName, BindingFlags flags, IConverter[] converters,
            out MethodInfo method)
        {
            var methods = type.GetMethods(flags)
                .Where(p => string.Equals(methodName, p.Name))
                .OfType<MethodBase>()
                .ToArray();

            method = methods.GetMethod(converters) as MethodInfo;
            return method != null;
        }

        public static bool TryGetConstructor(this Type type,
            IConverter[] converters,
            out ConstructorInfo ctor)
        {
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OfType<MethodBase>()
                .ToArray();
            if (ctors.Length == 0)
                ctors = new MethodBase[] { type.GetConstructor(Type.EmptyTypes) };

            ctor = ctors.GetMethod(converters) as ConstructorInfo;
            return ctor != null;
        }

        private static MethodBase GetMethod(this MethodBase[] methods, IConverter[] converters)
        {
            var length = methods.Length;

            var bestScore = int.MaxValue;
            var indexMatched = -1;
            for (var i = 0; i < length; i++)
            {
                // Check if the argument types match
                var parameters = methods[i].GetParameters();
                if (parameters.Length != converters.Length)
                    continue;

                // Compute a score about type match
                var match = true;
                var score = 0;
                for (var j = 0; j < converters.Length; j++)
                {
                    var parameterType = parameters[j].ParameterType.Extract();
                    var types = converters[j].GetClrTypes();

                    var found = false;
                    for (var k = 0; k < types.Length; k++)
                    {
                        if (types[k] != parameterType)
                            continue;

                        score += k;
                        found = true;
                        break;
                    }

                    #region Manage Enums
                    // Todo: Does exist a better way to match an enum type ??
                    if (parameterType.IsEnum && types.Contains(typeof (string)))
                    {
                        score += types.Length;
                        found = true;
                    }
                    else if (parameterType.IsEnumArray() && types.Contains(typeof (string[])))
                    {
                        score += types.Length + 1;
                        found = true;
                    }
                    #endregion

                    // Manage null value from R
                    if (types == NullConverter.Types)
                        found = true;

                    if (found) continue;

                    match = false;
                    break;
                }

                if (!match || score >= bestScore)
                    continue;

                bestScore = score;
                indexMatched = i;
            }

            if (indexMatched < 0)
                return null;

            return methods[indexMatched];
        }

        public static object Call(this MethodInfo method, object instance, IConverter[] converters)
        {
            var length = converters.Length;
            var args = new object[length];
            var parameters = method.GetParameters();

            for (var i = 0; i < length; i++)
                args[i] = converters[i].Convert(parameters[i].ParameterType.Extract());

            return method.Invoke(instance, args);
        }

        public static object Call(this ConstructorInfo ctor, IConverter[] converters)
        {
            var length = converters.Length;
            var args = new object[length];
            var parameters = ctor.GetParameters();

            for (var i = 0; i < length; i++)
                args[i] = converters[i].Convert(parameters[i].ParameterType.Extract());

            return ctor.Invoke(args);
        }

        public static bool IsEnumArray(this Type type)
        {
            return type.IsArray && type.GetElementType().IsEnum;
        }

        public static Type Extract(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
                return type.GetGenericArguments()[0];
            return type;
        }

        public static Type GetType(string typeName)
        {
            Type type;
            string errorMsg;
            if (TryGetType(typeName, out type, out errorMsg))
                return type;

            logger.ErrorFormat("Unable to get type: {0}, {1}", typeName, errorMsg);
            return null;
        }
    }
}
