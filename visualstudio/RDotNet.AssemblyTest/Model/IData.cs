using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using RDotNet.ClrProxy.R6;

namespace RDotNet.AssemblyTest.Model
{
    public interface IData
    {
    }

    public class Sample1 : IData
    {
        public Sample2 Sample2 { get; set; }

        public Sample2[] ArrayOfSample2 { get; set; }

        public List<Sample2> ListOfSample2 { get; set; }

        public void MethodNoReturn() { }
        public void MethofNoReturn(string arg1) { }

        public Sample2 MethodReturnSample2(string name)
        {
            return new Sample2 { Name = name };
        }

        public List<Sample2> MethodReturnListOfSample2(Sample2[] array)
        {
            return array.ToList();
        }

        public Sample2[] MethodReturnArrayOfSample2(List<Sample2> list)
        {
            return list.ToArray();
        }

        public string MethodReturnArgument(string argument)
        {
            return argument;
        }

        [Browsable(false)]
        public void InvisibleMethod()
        {
            
        }
    }

    public class Sample2
    {
        [Category("Mandatory")]
        public string Name { get; set; }

        [Category("Mandatory")]
        public int Id { get; set; }

        [Category("Parameters")]
        [Description("This is the given price")]
        public double Price { get; set; }

        [Category("Parameters")]
        public DateTime Timestamp { get; set; }

        [Category("Parameters")]
        public TimeSpan Elapsed { get; set; }

        [Category("Parameters")]
        public List<double> ListOfDouble { get; set; }

        [Category("Parameters")]
        public double[] ArrayOfDouble { get; set; }

        [Browsable(false)]
        public double InvisibleProperty { get; set; }
    }

    public class Sample3 : IData
    {
        private readonly string name;

        public string Name { get { return name; } }

        public Sample3(string name)
        {
            this.name = name;
        }
    }

    public class Sample4 : Sample3
    {
        public int Id { get; private set; }

        public Sample4(string name, int id) : base(name)
        {
            Id = id;
        }
    }

    public class Sample5 : Sample3
    {
        public string Name2 { get; set; }

        public double Price { get; set; }

        public Sample5(string name, string name2) : base(name)
        {
            Name2 = name2;
        }

        [R6Ctor]
        public Sample5(string name, double price)
            : base(name)
        {
            Price = price;
        }
    }
}
