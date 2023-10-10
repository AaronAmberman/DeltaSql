using SimpleLogger;
using System;

namespace DeltaSql.Services
{
    internal interface ILoggingService
    {
        #region Properties

        Logger Logger { get; }

        #endregion

        #region Events

        event EventHandler<(LogLevel LogLevel, string Message)> LogEntry;

        #endregion

        #region Methods

        void Debug(string message);
        void Error(string message);
        void Fatal(string message);
        void Info(string message);
        void SetLogPath();
        void SetLogPath(string path);
        void Trace(string message);
        void Warning(string message);

        #endregion
    }
}
