using System;
using System.Reflection;
using System.Runtime.InteropServices;
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

        private static void FormatException(StringBuilder sb, Exception exception)
        {
            while (exception != null)
            {
                var sehe = exception as SEHException;
                if (sehe != null)
                    sb.Append("\nCaught an SEHException, but no additional information is available via ErrorMessageProvider\n");

                AppendException(sb, exception);

                var rtle = exception as ReflectionTypeLoadException;
                if (rtle != null)
                {
                    foreach (var e in rtle.LoaderExceptions)
                        AppendException(sb, e);
                }

                exception = exception.InnerException;
            }
        }

        private static void AppendException(StringBuilder builder, Exception ex)
        {
            // Note that if using Environment.NewLine below instead of "\n", the rgui prompt is losing it
            // Actually even with the latter it is, but less so. Annoying.
            builder.AppendFormat("{0}Type:    {1}{0}Message: {2}{0}Method:  {3}{0}Stack trace:{0}{4}{0}{0}",
                "\n", ex.GetType(), ex.Message, ex.TargetSite, ex.StackTrace);
            // See whether this helps with the Rgui prompt:
        }

        private static string ToUnixNewline(string result)
        {
            return result.Replace("\r\n", "\n");
        }

        #endregion
    }
}
