using System;
using System.IO;
using System.Reflection;
using System.Text;
using RDotNet.ClrProxy.Converters;
using RDotNet.ClrProxy.Converters.RDotNet;
using RDotNet.ClrProxy.Loggers;

namespace RDotNet.ClrProxy
{
    public class ClrProxy
    {
        private static readonly ILogger logger = Logger.Instance;

        #region Mange Data converter

        private static IDataConverter dataConverter = RDotNetDataConverter.Instance;

        public static IDataConverter DataConverter
        {
            get { return dataConverter; }
            set { dataConverter = value ?? RDotNetDataConverter.Instance; }
        }

        #endregion

        public static Assembly LoadAssembly(string pathOrAssemblyName)
        {
            logger.DebugFormat("[LoadAssembly] Path or AssemblyName: {0}", pathOrAssemblyName);

            if (string.IsNullOrEmpty(pathOrAssemblyName))
                return null;

            try
            {
                var filePath = pathOrAssemblyName.Replace("/", "\\");
                if (File.Exists(filePath))
                {
                    var assemblyName = new FileInfo(filePath).Name;
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (string.Equals(assembly.ManifestModule.Name, assemblyName))
                            return assembly;
                    }

                    return Assembly.LoadFrom(filePath);
                }

                if (pathOrAssemblyName.IsFullyQualifiedAssemblyName())
                    return Assembly.Load(pathOrAssemblyName);

                throw new FileLoadException(string.Format("Unable to load assembly: {0}", pathOrAssemblyName));
            }
            catch (Exception e)
            {
                logger.Error("[LoadAssembly]", e);
                LastException = Format(e);
                throw;
            }
        }

        public static object CallStaticMethod(string typeName, string methodName, long[] argumentsAddresses)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
            
            logger.DebugFormat("[CallStaticMethod] TypeName: {0}, MethodName: {1}", typeName, methodName);
            
            try
            {
                Type type;
                string errorMsg;
                if (!typeName.TryGetType(out type, out errorMsg))
                    throw new TypeAccessException(errorMsg);

                var length = argumentsAddresses == null ? 0 : argumentsAddresses.Length;
                var converters = new IConverter[length];
                for (var i = 0; i < length; i++)
// ReSharper disable PossibleNullReferenceException
                    converters[i] = dataConverter.GetConverter(argumentsAddresses[i]);
// ReSharper restore PossibleNullReferenceException

                MethodInfo method;
                if (!type.TryGetMethod(methodName, flags, converters, out method))
                    throw new MissingMethodException(string.Format("Method not found, Type: {0}, Method: {1}", typeName, methodName));

                var result = method.Call(null, converters);
                return dataConverter.ConvertBack(method.ReturnType, result);
            }
            catch (Exception e)
            {
                logger.Error("[CallStaticMethod]", e);
                LastException = Format(e);
                throw;
            }
        }

        public static object GetStaticProperty(string typeName, string propertyName)
        {
            logger.DebugFormat("[GetStaticProperty] TypeName: {0}, PropertyName: {1}", typeName, propertyName);

            try
            {
                Type type;
                string errorMsg;
                if (!typeName.TryGetType(out type, out errorMsg))
                    throw new TypeAccessException(errorMsg);

                var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                    throw new MissingMemberException(string.Format("Static property {0} not found for Type: {1}", propertyName, type.FullName));
                if (!property.CanRead)
                    throw new InvalidOperationException(string.Format("Static property {0} can't be get for Type: {1}", propertyName, type.FullName));

                var result = property.GetGetMethod().Call(null, new IConverter[0]);
                return dataConverter.ConvertBack(property.PropertyType, result);
            }
            catch (Exception e)
            {
                logger.Error("[GetStaticProperty]", e);
                LastException = Format(e);
                throw;
            }
        }

        public static void SetStaticProperty(string typeName, string propertyName, long argumentAddresse)
        {
            logger.DebugFormat("[SetStaticProperty] TypeName: {0}, PropertyName: {1}", typeName, propertyName);

            try
            {
                Type type;
                string errorMsg;
                if (!typeName.TryGetType(out type, out errorMsg))
                    throw new TypeAccessException(errorMsg);

                var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                    throw new MissingMemberException(string.Format("Static property {0} not found for Type: {1}", propertyName, type.FullName));
                if (!property.CanWrite)
                    throw new InvalidOperationException(string.Format("Static property {0} can't be set for Type: {1}", propertyName, type.FullName));

                var converters = new[] { dataConverter.GetConverter(argumentAddresse) };

                property.GetSetMethod().Call(null, converters);
            }
            catch (Exception e)
            {
                logger.Error("[SetStaticProperty]", e);
                LastException = Format(e);
                throw;
            }
        }

        public static object CreateInstance(string typeName, long[] argumentsAddresses)
        {
            logger.DebugFormat("[CreateInstance] TypeName: {0}", typeName);

            try
            {
                Type type;
                string errorMsg;
                if (!typeName.TryGetType(out type, out errorMsg))
                    throw new TypeAccessException(errorMsg);

// ReSharper disable PossibleNullReferenceException
                var length = argumentsAddresses == null ? 0 : argumentsAddresses.Length;
                var converters = new IConverter[length];
                for (var i = 0; i < length; i++)
                    converters[i] = dataConverter.GetConverter(argumentsAddresses[i]);
// ReSharper restore PossibleNullReferenceException

                ConstructorInfo ctor;
                if (!type.TryGetConstructor(converters, out ctor))
                    throw new MissingMemberException(string.Format("Constructor not found for Type: {0}", typeName));

                var result = ctor.Call(converters);
                return dataConverter.ConvertBack(type, result);
            }
            catch (Exception e)
            {
                logger.Error("[CreateInstance]", e);
                LastException = Format(e);
                throw;
            }
        }

        public static void DisposeInstance(long address)
        {
            logger.Debug("[DisposeInstance]");

            try
            {
                var converter = dataConverter.GetConverter(address) as IDisposable;
                if (converter != null)
                    converter.Dispose();
            }
            catch (Exception e)
            {
                logger.Error("[DisposeInstance]", e);
                LastException = Format(e);
                throw;
            }
        }

        public static object CallMethod(object instance, string methodName, long[] argumentsAddresses)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            logger.DebugFormat("[CallMethod] Instance: {0}, MethodName: {1}", instance, methodName);

            try
            {
                if (instance == null)
                    throw new ArgumentNullException("instance");

                var type = instance.GetType();

// ReSharper disable PossibleNullReferenceException
                var length = argumentsAddresses == null ? 0 : argumentsAddresses.Length;
                var converters = new IConverter[length];
                for (var i = 0; i < length; i++)
                    converters[i] = dataConverter.GetConverter(argumentsAddresses[i]);
// ReSharper restore PossibleNullReferenceException

                MethodInfo method;
                if (!type.TryGetMethod(methodName, flags, converters, out method))
                    throw new MissingMethodException(string.Format("Method not found for Type: {0}, Method: {1}", type, methodName));

                var result = method.Call(instance, converters);
                return dataConverter.ConvertBack(method.ReturnType, result);
            }
            catch (Exception e)
            {
                logger.Error("[CallMethod]", e);
                LastException = Format(e);
                throw;
            }
        }

        public static object GetProperty(object instance, string propertyName)
        {
            logger.DebugFormat("[GetProperty] Instance: {0}, PropertyName: {1}", instance, propertyName);

            try
            {
                if (instance == null)
                    throw new ArgumentNullException("instance");

                var type = instance.GetType();

                var property = type.GetProperty(propertyName);
                if (property == null)
                    throw new MissingMemberException(string.Format("Property {0} not found for Type: {1}", propertyName, type.FullName));
                
                if (!property.CanRead)
                    throw new InvalidOperationException(string.Format("Property {0} can't be get for Type: {1}", propertyName, type.FullName));

                var result = property.GetGetMethod().Call(instance, new IConverter[0]);
                return dataConverter.ConvertBack(property.PropertyType, result);
            }
            catch (Exception e)
            {
                logger.Error("[GetProperty]", e);
                LastException = Format(e);
                throw;
            }
        }

        public static void SetProperty(object instance, string propertyName, long argumentAddresse)
        {
            logger.DebugFormat("[SetProperty] Instance: {0}, PropertyName: {1}", instance, propertyName);

            try
            {
                if (instance == null)
                    throw new ArgumentNullException("instance");

                var type = instance.GetType();

                var property = type.GetProperty(propertyName);
                if (property == null)
                    throw new MissingMemberException(string.Format("Property {0} not found for Type: {1}", propertyName, type.FullName));
                
                if (!property.CanWrite)
                    throw new InvalidOperationException(string.Format("Property {0} can't be set for Type: {1}", propertyName, type.FullName));

                var converters = new[] {dataConverter.GetConverter(argumentAddresse)};

                property.GetSetMethod().Call(instance, converters);
            }
            catch (Exception e)
            {
                logger.Error("[SetProperty]", e);
                LastException = Format(e);
                throw;
            }
        }

        public static string LastException { get; private set; }

        private static readonly StringBuilder builder = new StringBuilder();
        private static string Format(Exception e)
        {
            AbstractLogger.FormatException(builder, e);
            var result = builder.ToString();
            builder.Clear();
            return result;
        }
    }
}

