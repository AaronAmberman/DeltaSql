using SimpleLogger;
using System;

namespace DeltaSql.Services
{
    internal class LoggingService
    {
        #region Properties

        public Logger Logger { get; }

        #endregion

        #region Events

        public event EventHandler<(LogLevel, string)> LogEntry;

        #endregion

        #region Constructors

        public LoggingService(Logger logger)
        {
            Logger = logger;
        }

        #endregion

        #region Methods

        public void Debug(string message)
        {
            Logger?.Debug(message);

            LogEntry?.Invoke(this, (LogLevel.Debug, message));
        }

        public void Error(string message)
        {
            Logger?.Error(message);

            LogEntry?.Invoke(this, (LogLevel.Error, message));
        }

        public void Fatal(string message)
        {
            Logger?.Fatal(message);

            LogEntry?.Invoke(this, (LogLevel.Fatal, message));
        }

        public void Info(string message)
        {
            Logger?.Info(message);

            LogEntry?.Invoke(this, (LogLevel.Info, message));
        }

        public void Trace(string message)
        {
            Logger?.Trace(message);

            LogEntry?.Invoke(this, (LogLevel.Trace, message));
        }

        public void Warning(string message)
        {
            Logger?.Warning(message);

            LogEntry?.Invoke(this, (LogLevel.Warning, message));
        }

        #endregion
    }
}
