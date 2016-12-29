using System;
using System.Text;

namespace RDotNet.ClrProxy.Loggers
{
    public abstract class AbstractLogger : ILogger
    {
        private readonly string name;
        private LogLevel level;
        private readonly StringBuilder buffer = new StringBuilder();

        protected AbstractLogger(string name)
        {
            this.name = name;
            UpdateLevel(LogLevel.Info);
        }

        #region Implementation of ILogger

        public string Name { get { return name; } }

        public virtual LogLevel Level
        {
            get { return level; }
            set
            {
                if (level == value) return;
                UpdateLevel(value);
            }
        }

        public bool IsDebugEnabled { get; private set; }

        public void Debug(string message)
        {
            if (!IsDebugEnabled) return;

            DebugOverride(message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (!IsDebugEnabled) return;

            buffer.Clear();
            buffer.AppendFormat(format, args);
            Debug(buffer.ToString());
        }

        public void Debug(string message, Exception e)
        {
            if (!IsDebugEnabled) return;

            buffer.Clear();
            buffer.AppendLine(message);
            FormatException(buffer, e);
            Debug(buffer.ToString());
        }

        public bool IsInfoEnabled { get; private set; }

        public void Info(string message)
        {
            if (!IsInfoEnabled) return;

            InfoOverride(message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (!IsInfoEnabled) return;

            buffer.Clear();
            buffer.AppendFormat(format, args);
            Info(buffer.ToString());
        }

        public void Info(string message, Exception e)
        {
            if (!IsInfoEnabled) return;

            buffer.Clear();
            buffer.AppendLine(message);
            FormatException(buffer, e);
            Info(buffer.ToString());
        }

        public bool IsWarnEnabled { get; private set; }

        public void Warn(string message)
        {
            if (!IsWarnEnabled) return;

            WarnOverride(message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (!IsWarnEnabled) return;

            buffer.Clear();
            buffer.AppendFormat(format, args);
            Warn(buffer.ToString());
        }

        public void Warn(string message, Exception e)
        {
            if (!IsWarnEnabled) return;

            buffer.Clear();
            buffer.AppendLine(message);
            FormatException(buffer, e);
            Warn(buffer.ToString());
        }

        public bool IsErrorEnabled { get; private set; }

        public void Error(string message)
        {
            if (!IsErrorEnabled) return;

            ErrorOverride(message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (!IsErrorEnabled) return;

            buffer.Clear();
            buffer.AppendFormat(format, args);
            Error(buffer.ToString());
        }

        public void Error(string message, Exception e)
        {
            if (!IsErrorEnabled) return;

            buffer.Clear();
            buffer.AppendLine(message);
            FormatException(buffer, e);
            Error(buffer.ToString());
        }

        #endregion

        protected abstract void DebugOverride(string message);
        protected abstract void InfoOverride(string message);
        protected abstract void WarnOverride(string message);
        protected abstract void ErrorOverride(string message);

        private void UpdateLevel(LogLevel value)
        {
            level = value;
            IsDebugEnabled = value <= LogLevel.Debug;
            IsInfoEnabled = value <= LogLevel.Info;
            IsWarnEnabled = value <= LogLevel.Warn;
            IsErrorEnabled = value <= LogLevel.Error;
        }

        #region Format exception

        public static void FormatException(StringBuilder sb, Exception e)
        {
            while (e != null)
            {
                sb.AppendLine();
                sb.AppendFormat("[Message] {0}", e.Message);
                sb.AppendLine();
                sb.AppendFormat("[StackTrace] {0}", e.StackTrace);
                sb.AppendLine();

                e = e.InnerException;
            }
        }

        #endregion
    }
}
