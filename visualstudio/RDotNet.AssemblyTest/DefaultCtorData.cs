namespace RDotNet.AssemblyTest
{
    public class DefaultCtorData
    {
        public string Name { get; set; }

        public int[] Integers { get; set; }

        public OneCtorData OneCtorData { get; set; }
    }

    public class OneCtorData
    {
        private readonly int id;

        public OneCtorData(int id)
        {
            this.id = id;
        }

        public override string ToString()
        {
            return string.Format("{0} #{1}", base.ToString(), id);
        }
    }

    public class ManyCtorData
    {
        private readonly int id;
        private readonly string name;

        public ManyCtorData()
        {
            this.id = -1;
            this.name = "Default ctor #" + id;
        }

        public ManyCtorData(int id)
        {
            this.id = id;
            this.name = "Integer ctor #" + id;
        }

        public ManyCtorData(string name)
        {
            this.name = "String Ctor " + name;
        }

        public override string ToString()
        {
            return string.Format("{0} Name={1}", base.ToString(), name);
        }
    }
}
