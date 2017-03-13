using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using RDotNet.ClrProxy;

namespace RDotNet.ClrProxyTests.R6
{
    public class PropertiesOnly
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public double Price { get; set; }

        public DateTime Timestamp { get; set; }

        public TimeSpan Elapsed { get; set; }

        public double GetOnly { get { return 1.23; } }

        public double SetOnly { private get; set; }
    }

    public class FullClass
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public double Price { get; set; }

        public DateTime Timestamp { get; set; }

        public TimeSpan Elapsed { get; set; }

        public void Method1()
        {
            
        }

        public int Method2()
        {
            return 42;
        }

        public int Method2(int arg1)
        {
            return arg1;
        }

        public int Add(int x, int y)
        {
            return x + y;
        }
    }

    [TestFixture]
    public class R6ClassGeneratorTests
    {
        [Test]
        public void TestGeneratePropertiesOnly()
        {
            var sb = new StringBuilder();
            sb.AppendLine("library(R6)");
            sb.AppendLine("library(r.net)");
            sb.AppendLine();

            sb.GenerateR6Class(typeof (PropertiesOnly));
            sb.AppendLine();
            sb.GenerateR6Class(typeof(FullClass));
            sb.AppendLine();

            File.WriteAllText("R6.R", sb.ToString());
        }
    }
}
