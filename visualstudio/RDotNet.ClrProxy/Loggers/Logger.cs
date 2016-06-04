using System.Collections.Generic;

namespace RDotNet.ClrProxy.Loggers
{
    public class Logger : AbstractLogger
    {
        #region Singleton

        private static readonly Logger instance = new Logger();
        public static Logger Instance { get { return instance; } }

        #endregion

        private readonly List<ILogger> loggers = new List<ILogger>();

        private Logger() : base("Main Logger")
        {
            loggers.Add(new RConsoleLogger());
        }

        /// <summary>
        /// Add a logger.
        /// </summary>
        /// <param name="logger">Logger to add</param>
        public void AddLogger(ILogger logger)
        {
            if (logger == null) return;

            var count = loggers.Count;
            for (var i = 0; i < count; i++)
            {
                if (string.Equals(logger.Name, loggers[i].Name))
                {
                    loggers[i] = logger;
                    return;
                }
            }

            loggers.Add(logger);
        }

        /// <summary>
        /// Remove a logger.
        /// </summary>
        /// <param name="logger">Logger to remove</param>
        public void RemoveLogger(ILogger logger)
        {
            if (logger == null) return;
            RemoveLogger(logger.Name);
        }

        /// <summary>
        /// Remove a logger by name.
        /// </summary>
        /// <param name="name">Logger name to remove.</param>
        public void RemoveLogger(string name)
        {
            loggers.RemoveAll(p => string.Equals(p.Name, name));
        }

        #region Overrides of AbstractLogger

        public override LogLevel Level
        {
            get { return base.Level; }
            set
            {
                base.Level = value;

                var count = loggers.Count;
                for (var i = 0; i < count; i++)
                    loggers[i].Level = value;
            }
        }

        protected override void DebugOverride(string message)
        {
            var count = loggers.Count;
            for (var i = 0; i < count; i++)
                loggers[i].Debug(message);
        }

        protected override void InfoOverride(string message)
        {
            var count = loggers.Count;
            for (var i = 0; i < count; i++)
                loggers[i].Info(message);
        }

        protected override void WarnOverride(string message)
        {
            var count = loggers.Count;
            for (var i = 0; i < count; i++)
                loggers[i].Warn(message);
        }

        protected override void ErrorOverride(string message)
        {
            var count = loggers.Count;
            for (var i = 0; i < count; i++)
                loggers[i].Error(message);
        }

        #endregion
    }
}
