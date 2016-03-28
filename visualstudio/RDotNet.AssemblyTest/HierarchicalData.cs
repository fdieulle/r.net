namespace RDotNet.AssemblyTest
{
    public class BaseData : IBase
    {

    }

    public class DataLvl1 : BaseData, ILvl1
    {
        
    }

    public class DataLvl2 : DataLvl1, ILvl2, IOther
    {
        
    }

    public interface IBase{}
    public interface ILvl1 : IBase { }
    public interface ILvl2 : ILvl1 { }
    public interface IOther { }
}
