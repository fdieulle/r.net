using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using RDotNet.AssemblyTest;
using RDotNet.ClrProxy;
using RDotNet.ClrProxy.Converters;

namespace RDotNet.ClrProxyTests
{
    [TestFixture]
    public class ReflectionTests
    {
        [Test]
        public void TestTryGetMethod()
        {
            var type = typeof (StaticClass);
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
            const string methodName = "SameMethodName";

            MethodInfo method;
            var types = new[] { C(typeof(double), typeof(double[])) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(double)), null));

            types = new[] { C(typeof(int), typeof(int[])) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(int)), null));

            types = new[] { C(typeof(double[])) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(double[])), null));

            types = new[] { C(typeof(int[])) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(int[])), null));

            types = new[] { C(typeof(double), typeof(double[])), C(typeof(int), typeof(int[])) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(double), typeof(int)), null));

            types = new[] { C(typeof(double[])), C(typeof(int[])) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(double[]), typeof(int[])), null));

            types = new[] { C(typeof(double[])), C(typeof(int), typeof(int[])) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(double[]), typeof(int)), null));

            types = new[] { C(typeof(double), typeof(double[])), C(typeof(int[])) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(double), typeof(int[])), null));

            types = new[] { C(typeof(string), typeof(string[])) };
            Assert.IsFalse(type.TryGetMethod(methodName, flags, types, out method));
            Assert.IsNull(method);
        }

        [Test]
        public void TestTryGetCustomMethods()
        {
            var type = typeof(StaticClass);
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
            
            MethodInfo method;
            var methodName = "OptionalArguments";

            var types = new[] { C(typeof(string), typeof(string[])), C(typeof(int), typeof(int[])), C(typeof(bool)) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(string[]), typeof(int[]), typeof(bool)), null));

            types = new[] { C(typeof(string), typeof(string[])), C(NullConverter.Types), C(typeof(bool)) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(string[]), typeof(int[]), typeof(bool)), null));

            methodName = "NullValueFromR";

            types = new[] { C(NullConverter.Types), C(NullConverter.Types), C(NullConverter.Types) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(string[]), typeof(object), typeof(bool)), null));

            method.Invoke(null, new object[] {null, null, null});
        }

        [Test]
        public void TestTryGetMethodWithEnumArgument()
        {
            var type = typeof(StaticClass);
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
            
            var methodName = "CallWithEnum";
            MethodInfo method;
            var types = new[] { C(typeof(string), typeof(string[])) };
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(EnumSample)), null));

            methodName = "CallWithEnumVector";
            Assert.IsTrue(type.TryGetMethod(methodName, flags, types, out method));
            Assert.AreEqual(method, type.GetMethod(methodName, flags, null, T(typeof(EnumSample[])), null));
        }

        [Test]
        public void TestTryGetCtor()
        {
            var type = typeof(DefaultCtorData);

            ConstructorInfo ctor;
            type.TryGetConstructor(new IConverter[0], out ctor);
            Assert.IsNotNull(ctor);

            var typeName = typeof (OneCtorData).FullName;
            string errorMsg;
            
            Assert.IsTrue(typeName.TryGetType(out type, out errorMsg));
            Assert.IsTrue(string.IsNullOrEmpty(errorMsg));
            var converters = new[] { C(typeof(int), typeof(int[])) };            
            Assert.IsTrue(type.TryGetConstructor(converters, out ctor));
            Assert.AreEqual(type.GetConstructor(T(typeof(int))), ctor);

            typeName = typeof(ManyCtorData).FullName;
            Assert.IsTrue(typeName.TryGetType(out type, out errorMsg));
            Assert.IsTrue(string.IsNullOrEmpty(errorMsg));
            
            converters = new[] { C(typeof(string), typeof(string[])) };
            Assert.IsTrue(type.TryGetConstructor(converters, out ctor));
            Assert.AreEqual(type.GetConstructor(T(typeof(string))), ctor);

            converters = new[] { C(typeof(int), typeof(int[])) };
            Assert.IsTrue(type.TryGetConstructor(converters, out ctor));
            Assert.AreEqual(type.GetConstructor(T(typeof(int))), ctor);

            converters = new IConverter[0];
            Assert.IsTrue(type.TryGetConstructor(converters, out ctor));
            Assert.AreEqual(type.GetConstructor(T()), ctor);
        }

        [Test]
        public void TestGetFullTypeHierarchy()
        {
            var types = typeof (BaseData).GetFullHierarchy();
            Check(types, typeof(BaseData), typeof(IBase), typeof(object));

            types = typeof(DataLvl1).GetFullHierarchy();
            Check(types, typeof(DataLvl1), typeof(ILvl1), typeof(BaseData), typeof(IBase), typeof(object));

            types = typeof(DataLvl2).GetFullHierarchy();
            Check(types, typeof(DataLvl2), typeof(ILvl2), typeof(IOther), typeof(DataLvl1), typeof(ILvl1), typeof(BaseData), typeof(IBase), typeof(object));

            types = typeof(IBase).GetFullHierarchy();
            Check(types, typeof(IBase), typeof(object));

            types = typeof(ILvl1).GetFullHierarchy();
            Check(types, typeof(ILvl1), typeof(IBase), typeof(object));

            types = typeof(ILvl2).GetFullHierarchy();
            Check(types, typeof(ILvl2), typeof(ILvl1), typeof(IBase), typeof(object));

            types = typeof(IOther).GetFullHierarchy();
            Check(types, typeof(IOther), typeof(object));

            types = typeof(List<object>).GetFullHierarchy();
            Check(types,
                typeof(List<object>),
                typeof(IList<object>),
                typeof(IList),
                typeof(IReadOnlyList<object>),
                typeof(ICollection<object>),
                typeof(ICollection),
                typeof(IReadOnlyCollection<object>),
                typeof(IEnumerable<object>),
                typeof(IEnumerable),
                typeof(object));
            
            types = typeof(IList<object>).GetFullHierarchy();
            Check(types,
                typeof(IList<object>),
                typeof(ICollection<object>),
                typeof(IEnumerable<object>),
                typeof(IEnumerable),
                typeof(object));

            types = typeof(Dictionary<string, object>).GetFullHierarchy();
            Check(types,
                typeof(Dictionary<string, object>),
                typeof(IDictionary<string, object>),
                typeof(IDictionary),
                typeof(IReadOnlyDictionary<string, object>),
                typeof(ISerializable),
                typeof(IDeserializationCallback),
                typeof(ICollection<KeyValuePair<string, object>>),
                typeof(ICollection),
                typeof(IReadOnlyCollection<KeyValuePair<string, object>>),
                typeof(IEnumerable<KeyValuePair<string, object>>),
                typeof(IEnumerable),
                typeof(object));
        }

        [Test]
        public void UnionTests()
        {
            var left = typeof(List<OneCtorData>).GetFullHierarchy();
            var rights = typeof(List<object>).GetFullHierarchy();
            var types = left.Union(rights);
            Check(types,
                typeof(List<OneCtorData>),
                typeof(IList<OneCtorData>),
                typeof(List<object>),
                typeof(IList<object>),
                typeof(IList),
                typeof(IReadOnlyList<OneCtorData>),
                typeof(ICollection<OneCtorData>),
                typeof(IReadOnlyList<object>),
                typeof(ICollection<object>),
                typeof(ICollection),
                typeof(IReadOnlyCollection<OneCtorData>),
                typeof(IEnumerable<OneCtorData>),
                typeof(IReadOnlyCollection<object>),
                typeof(IEnumerable<object>),
                typeof(IEnumerable),
                typeof(object));

            left = typeof (Dictionary<string, object>).GetFullHierarchy();
            rights = typeof (List<object>).GetFullHierarchy();
            types = left.Union(rights);

            Check(types,
                typeof(Dictionary<string, object>),
                typeof(IDictionary<string, object>),
                typeof(IDictionary),
                typeof(IReadOnlyDictionary<string, object>),
                typeof(ISerializable),
                typeof(IDeserializationCallback),
                typeof(ICollection<KeyValuePair<string, object>>),
                typeof(List<object>),
                typeof(IList<object>),
                typeof(IList),
                typeof(IReadOnlyList<object>),
                typeof(ICollection<object>),
                typeof(ICollection),
                typeof(IReadOnlyCollection<KeyValuePair<string, object>>),
                typeof(IEnumerable<KeyValuePair<string, object>>),
                typeof(IReadOnlyCollection<object>),
                typeof(IEnumerable<object>),
                typeof(IEnumerable),
                typeof(object));
        }

        public static void Check(Type[] types, params Type[] expected)
        {
            Assert.AreEqual(expected.Length, types.Length, Format(expected, types));
            for(var i=0; i< expected.Length; i++)
                Assert.AreEqual(expected[i], types[i]);
        }

        private static string Format(Type[] expected, Type[] butWas)
        {
            return string.Format("Expected:{0}{1}{0}But was:{0}{2}", Environment.NewLine,
                          string.Join(Environment.NewLine, expected.Select(p => p.FullName)),
                          string.Join(Environment.NewLine, butWas.Select(p => p.FullName)));
        }

        private static Type[] T(params Type[] types)
        {
            return types;
        }

        private static IConverter C(params Type[] types)
        {
            return new PrivateConverter(types);
        }

        private class PrivateConverter : IConverter
        {
            private readonly Type[] types;

            public PrivateConverter(Type[] types)
            {
                this.types = types;
            }

            #region Implementation of IConverter

            public Type[] GetClrTypes()
            {
                return types;
            }

            public object Convert(Type type)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

    }
}
