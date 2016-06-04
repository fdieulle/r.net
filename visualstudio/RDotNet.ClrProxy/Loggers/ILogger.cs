using System;

namespace RDotNet.ClrProxy.Loggers
{
    /// <summary>
    /// This interface define a logger with different level messages to log.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the logger name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the log level enabled.
        /// </summary>
        LogLevel Level { get; set; }

        /// <summary>
        /// Gets if Debug log level is enabled.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// Log a Debug level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Debug(string message);

        /// <summary>
        /// Log a Debug level message with a format syntax
        /// </summary>
        /// <param name="format">String format</param>
        /// <param name="args">ARguments to format.</param>
        void DebugFormat(string format, params object[] args);

        /// <summary>
        /// Log a Debug level message with its exception.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="e">Exception to log</param>
        void Debug(string message, Exception e);

        /// <summary>
        /// Gets if Info log level is enabled.
        /// </summary>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// Log an Info level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Info(string message);

        /// <summary>
        /// Log an Info level message with a format syntax
        /// </summary>
        /// <param name="format">String format</param>
        /// <param name="args">ARguments to format.</param>
        void InfoFormat(string format, params object[] args);

        /// <summary>
        /// Log an Info level message with its exception.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="e">Exception to log</param>
        void Info(string message, Exception e);

        /// <summary>
        /// Gets if Warn log level is enabled.
        /// </summary>
        bool IsWarnEnabled { get; }

        /// <summary>
        /// Log a Warn level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Warn(string message);

        /// <summary>
        /// Log a Warn level message with a format syntax
        /// </summary>
        /// <param name="format">String format</param>
        /// <param name="args">ARguments to format.</param>
        void WarnFormat(string format, params object[] args);

        /// <summary>
        /// Log a Warn level message with its exception.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="e">Exception to log</param>
        void Warn(string message, Exception e);

        /// <summary>
        /// Gets if Error log level is enabled.
        /// </summary>
        bool IsErrorEnabled { get; }

        /// <summary>
        /// Log an Error level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Error(string message);

        /// <summary>
        /// Log an Error level message with a format syntax
        /// </summary>
        /// <param name="format">String format</param>
        /// <param name="args">ARguments to format.</param>
        void ErrorFormat(string format, params object[] args);

        /// <summary>
        /// Log an Error level message with its exception.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="e">Exception to log</param>
        void Error(string message, Exception e);
    }
}
