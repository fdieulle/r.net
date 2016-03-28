using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using RDotNet.ClrProxy.Converters;
using RDotNet.ClrProxy.Converters.RDotNet;

namespace RDotNet.ClrProxy
{
    public class ClrProxy
    {
        // Todo found a better way to create it and allow to enrich it or change it
        private static readonly IDataConverter dataConverter = RDotNetDataConverter.Instance;

        public static Assembly LoadAssembly(string pathOrAssemblyName)
        {
            if (string.IsNullOrEmpty(pathOrAssemblyName))
                return null;

            var filePath = pathOrAssemblyName.Replace("/", "\\");
            if (File.Exists(filePath))
                return Assembly.LoadFrom(filePath);

            if (pathOrAssemblyName.IsFullyQualifiedAssemblyName())
                return Assembly.Load(pathOrAssemblyName);

            Console.WriteLine("Unable to load assembly: {0}", pathOrAssemblyName);
            return null;
        }

        public static object CallStaticMethod(string typeName, string methodName, long[] argumentsAddresses)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
            Console.WriteLine("Type: {0}, Method: {1}", typeName, methodName);
            try
            {
                Type type;
                string errorMsg;
                if (!typeName.TryGetType(out type, out errorMsg))
                {
                    // Todo: Enqueue this message
                    Console.WriteLine(errorMsg);
                    return null;
                }

                var length = argumentsAddresses == null ? 0 : argumentsAddresses.Length;
                var converters = new IConverter[length];
                for (var i = 0; i < length; i++)
// ReSharper disable PossibleNullReferenceException
                    converters[i] = dataConverter.GetConverter(argumentsAddresses[i]);
// ReSharper restore PossibleNullReferenceException

                MethodInfo method;
                if (!type.TryGetMethod(methodName, flags, converters, out method))
                {
                    // Todo: Enqueue this message
                    Console.WriteLine("Method not found");
                    return null;
                }

                var result = method.Call(null, converters);
                return dataConverter.ConvertBack(method.ReturnType, result);
            }
            catch (Exception e)
            {
                // Todo: Format exception then enqueue the message
                Console.WriteLine(FormatException(e));
                return null;
            }
        }

        public static object CreateInstance(string typeName, long[] argumentsAddresses)
        {
            try
            {
                Type type;
                string errorMsg;
                if (!typeName.TryGetType(out type, out errorMsg))
                {
                    // Todo: Enqueue this message
                    Console.WriteLine(errorMsg);
                    return null;
                }

                var length = argumentsAddresses == null ? 0 : argumentsAddresses.Length;
                var converters = new IConverter[length];
                for (var i = 0; i < length; i++)
                    // ReSharper disable PossibleNullReferenceException
                    converters[i] = dataConverter.GetConverter(argumentsAddresses[i]);
                // ReSharper restore PossibleNullReferenceException

                ConstructorInfo ctor;
                if (!type.TryGetConstructor(converters, out ctor))
                {
                    // Todo: Enqueue this message
                    Console.WriteLine("Ctor not found");
                    return null;
                }

                var result = ctor.Call(converters);
                return dataConverter.ConvertBack(type, result);
            }
            catch (Exception e)
            {
                // Todo: Format exception then enqueue the message
                Console.WriteLine(FormatException(e));
                return null;
            }
        }

        public static object CallMethod(object instance, string methodName, long[] argumentsAddresses)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            
            try
            {
                if(instance == null)
                    throw new ArgumentNullException("instance");

                var type = instance.GetType();

                var length = argumentsAddresses == null ? 0 : argumentsAddresses.Length;
                var converters = new IConverter[length];
                for (var i = 0; i < length; i++)
// ReSharper disable PossibleNullReferenceException
                    converters[i] = dataConverter.GetConverter(argumentsAddresses[i]);
// ReSharper restore PossibleNullReferenceException

                MethodInfo method;
                if (!type.TryGetMethod(methodName, flags, converters, out method))
                {
                    // Todo: Enqueue this message
                    Console.WriteLine("Method not found");
                    return null;
                }

                var result = method.Call(instance, converters);
                return dataConverter.ConvertBack(method.ReturnType, result);
            }
            catch (Exception e)
            {
                // Todo: Format exception then enqueue the message
                Console.WriteLine(FormatException(e));
                return null;
            }
        }

        public static object GetProperty(object instance, string propertyName)
        {
            Console.WriteLine("GetProperty Instance: {0}, PropertyName: {1}", instance, propertyName);

            try
            {
                if (instance == null)
                    throw new ArgumentNullException("instance");

                var type = instance.GetType();

                var property = type.GetProperty(propertyName);
                if (property == null)
                {
                    // Todo: Enqueue this message
                    Console.WriteLine("Property {0} not found on type {1}", propertyName, type.FullName);
                    return null;
                }
                if (!property.CanRead)
                {
                    // Todo: Enqueue this message
                    Console.WriteLine("Property {0} can't be read on type {1}", propertyName, type.FullName);
                    return null;
                }

                var result = property.GetGetMethod().Call(instance, new IConverter[0]);
                return dataConverter.ConvertBack(property.PropertyType, result);
            }
            catch (Exception e)
            {
                // Todo: Format exception then enqueue the message
                Console.WriteLine(FormatException(e));
                return null;
            }
        }

        public static void SetProperty(object instance, string propertyName, long argumentAddresse)
        {
            Console.WriteLine("SetProperty Instance: {0}, PropertyName: {1}, Addresse: {2}", instance, propertyName, argumentAddresse);

            try
            {
                if (instance == null)
                    throw new ArgumentNullException("instance");

                var type = instance.GetType();

                var property = type.GetProperty(propertyName);
                if (property == null)
                {
                    // Todo: Enqueue this message
                    Console.WriteLine("Property {0} not found on type {1}", propertyName, type.FullName);
                    return;
                }
                if (!property.CanWrite)
                {
                    // Todo: Enqueue this message
                    Console.WriteLine("Property {0} can't be write on type {1}", propertyName, type.FullName);
                    return;
                }

                var converters = new[] {dataConverter.GetConverter(argumentAddresse)};

                property.GetSetMethod().Call(instance, converters);
            }
            catch (Exception e)
            {
                // Todo: Format exception then enqueue the message
                Console.WriteLine(FormatException(e));
            }
        }

        #region Format exception

        private static string FormatException(Exception exception)
        {
            var sb = new StringBuilder();

            while (exception != null)
            {
                var sehe = exception as SEHException;
                if (sehe != null)
                    sb.Append("\nCaught an SEHException, but no additional information is available via ErrorMessageProvider\n");

                AppendException(sb, exception);

                var rtle = exception as ReflectionTypeLoadException;
                if (rtle != null)
                {
                    foreach (var e in rtle.LoaderExceptions)
                        AppendException(sb, e);
                }

                exception = exception.InnerException;
            }

            return ToUnixNewline(sb.ToString());
        }

        private static void AppendException(StringBuilder builder, Exception ex)
        {
            // Note that if using Environment.NewLine below instead of "\n", the rgui prompt is losing it
            // Actually even with the latter it is, but less so. Annoying.
            builder.AppendFormat("{0}Type:    {1}{0}Message: {2}{0}Method:  {3}{0}Stack trace:{0}{4}{0}{0}",
                "\n", ex.GetType(), ex.Message, ex.TargetSite, ex.StackTrace);
            // See whether this helps with the Rgui prompt:
        }

        private static string ToUnixNewline(string result)
        {
            return result.Replace("\r\n", "\n");
        }

        #endregion
    }
}

