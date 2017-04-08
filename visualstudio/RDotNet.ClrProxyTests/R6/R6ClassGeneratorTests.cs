using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using RDotNet.AssemblyTest.Model;
using RDotNet.ClrProxy.R6;

namespace RDotNet.ClrProxyTests.R6
{
    [TestFixture]
    public class R6ClassGeneratorTests
    {
        [Test]
        public void TestGeneratePropertiesOnly()
        {
            const string folder = @"testDoc/R";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            R6Generator.GenerateR6Classes(new[]
                {
                    typeof(ClassWithDependencies).FullName, 
                    typeof(IData).FullName
                }, Path.Combine(folder, "R6-Generated.R"), withInheritedTypes:true);
        }

        [Test]
        public void Test()
        {
            var properties = typeof (List<object>).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(properties);
        }
    }
}
