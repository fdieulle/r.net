using System;
using System.IO;
using System.Reflection;
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

            logger.ErrorFormat("Unable to load assembly: {0}", pathOrAssemblyName);
            return null;
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
                {
                    logger.Error(errorMsg);
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
                    logger.ErrorFormat("Method not found, Type: {0}, Method: {1}", typeName, methodName);
                    return null;
                }

                var result = method.Call(null, converters);
                return dataConverter.ConvertBack(method.ReturnType, result);
            }
            catch (Exception e)
            {
                logger.Error("[CallStaticMethod]", e);
                return null;
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
                {
                    logger.Error(errorMsg);
                    return null;
                }

                var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                {
                    logger.ErrorFormat("Property {0} not found for Type: {1}", propertyName, type.FullName);
                    return null;
                }
                if (!property.CanRead)
                {
                    logger.ErrorFormat("Property {0} can't be read for Type: {1}", propertyName, type.FullName);
                    return null;
                }

                var result = property.GetGetMethod().Call(null, new IConverter[0]);
                return dataConverter.ConvertBack(property.PropertyType, result);
            }
            catch (Exception e)
            {
                logger.Error("[GetStaticProperty]", e);
                return null;
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
                {
                    logger.Error(errorMsg);
                    return;
                }

                var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                {
                    logger.ErrorFormat("Property {0} not found for Type: {1}", propertyName, type.FullName);
                    return;
                }
                if (!property.CanWrite)
                {
                    logger.ErrorFormat("Property {0} can't be written for Type: {1}", propertyName, type.FullName);
                    return;
                }

                var converters = new[] { dataConverter.GetConverter(argumentAddresse) };

                property.GetSetMethod().Call(null, converters);
            }
            catch (Exception e)
            {
                logger.Error("[SetStaticProperty]", e);
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
                {
                    logger.Error(errorMsg);
                    return null;
                }

// ReSharper disable PossibleNullReferenceException
                var length = argumentsAddresses == null ? 0 : argumentsAddresses.Length;
                var converters = new IConverter[length];
                for (var i = 0; i < length; i++)
                    converters[i] = dataConverter.GetConverter(argumentsAddresses[i]);
// ReSharper restore PossibleNullReferenceException

                ConstructorInfo ctor;
                if (!type.TryGetConstructor(converters, out ctor))
                {
                    logger.ErrorFormat("Constructor not found for Type: {0}", typeName);
                    return null;
                }

                var result = ctor.Call(converters);
                return dataConverter.ConvertBack(type, result);
            }
            catch (Exception e)
            {
                logger.Error("[CallStaticMethod]", e);
                return null;
            }
        }

        public static object CallMethod(object instance, string methodName, long[] argumentsAddresses)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            logger.DebugFormat("[CallMethod] Instance: {0}, MethodName: {1}", instance, methodName);

            try
            {
                if(instance == null)
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
                {
                    logger.ErrorFormat("Method not found for Type: {0}, Method: {1}", type, methodName);
                    return null;
                }

                var result = method.Call(instance, converters);
                return dataConverter.ConvertBack(method.ReturnType, result);
            }
            catch (Exception e)
            {
                logger.Error("[CallMethod]", e);
                return null;
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
                {
                    logger.ErrorFormat("Property {0} not found for Type: {1}", propertyName, type.FullName);
                    return null;
                }
                if (!property.CanRead)
                {
                    logger.ErrorFormat("Property {0} can't be read for Type: {1}", propertyName, type.FullName);
                    return null;
                }

                var result = property.GetGetMethod().Call(instance, new IConverter[0]);
                return dataConverter.ConvertBack(property.PropertyType, result);
            }
            catch (Exception e)
            {
                logger.Error("[GetProperty]", e);
                return null;
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
                {
                    logger.ErrorFormat("Property {0} not found for Type: {1}", propertyName, type.FullName);
                    return;
                }
                if (!property.CanWrite)
                {
                    logger.ErrorFormat("Property {0} can't be written for Type: {1}", propertyName, type.FullName);
                    return;
                }

                var converters = new[] {dataConverter.GetConverter(argumentAddresse)};

                property.GetSetMethod().Call(instance, converters);
            }
            catch (Exception e)
            {
                logger.Error("[SetProperty]", e);
            }
        }
    }
}

