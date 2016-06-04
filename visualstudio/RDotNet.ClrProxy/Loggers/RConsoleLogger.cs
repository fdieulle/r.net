using System;

namespace RDotNet.ClrProxy.Loggers
{
    public class RConsoleLogger : AbstractLogger
    {
        public RConsoleLogger() 
            : base("r.net") { }

        #region Overrides of AbstractLogger

        protected override void DebugOverride(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }

        protected override void InfoOverride(string message)
        {
            Console.WriteLine(message);
        }

        protected override void WarnOverride(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }

        protected override void ErrorOverride(string message)
        {
            Console.Error.WriteLine(message);
        }

        #endregion
    }
}
