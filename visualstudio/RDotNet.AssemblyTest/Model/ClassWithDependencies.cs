using System;
using System.Collections.Generic;

namespace RDotNet.AssemblyTest.Model
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

        public string[] ArrayOfString { get; set; }
        public List<string> ListOfString { get; set; }
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

    public class ClassWithDependencies
    {
        public PropertiesOnly PropertiesOnly { get; set; }

        public List<FullClass> ListOfFullClass { get; set; }

        public List<PropertiesOnly> MethodReturnListOfPropertiesOnly(int length)
        {
            var result = new List<PropertiesOnly>();
            for (var i = 0; i < length; i++)
                result.Add(new PropertiesOnly { Id = i + 1 });
            return result;
        }

        public int MethodTakeAListOfFullClass(List<FullClass> list)
        {
            return list.Count;
        }

        public void MethodWithSimpleArgsNoReturn(double arg0, string arg1)
        {

        }

        public bool MethodWithSimpleArgs(double arg0, string arg1)
        {
            return true;
        }

        public FullClass MethodReturnsFullClass(string name)
        {
            return new FullClass { Name = name };
        }

        public override string ToString()
        {
            return "An instance of Sample1";
        }
    }
}
