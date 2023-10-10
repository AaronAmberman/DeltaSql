using DeltaSql.Properties;
using SimpleLogger;
using System;
using System.IO;
using System.Reflection;

namespace DeltaSql.Services
{
    internal class LoggingService : ILoggingService
    {
        #region Properties

        public Logger Logger { get; }

        #endregion

        #region Events

        public event EventHandler<(LogLevel LogLevel, string Message)> LogEntry;

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

        public void SetLogPath()
        {
            SetLogPath(Settings.Default.LogPath);
        }

        public void SetLogPath(string path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path))
                    ServiceLocator.Instance.LoggingService.Logger.LogFile = Path.Combine(path, "DeltaSql.log");
                else
                {
                    string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    if (!string.IsNullOrWhiteSpace(location))
                        ServiceLocator.Instance.LoggingService.Logger.LogFile = Path.Combine(location, "DeltaSql.log");
                }
            }
            catch
            {
                // we cannot determine location for some reason, use desktop
                ServiceLocator.Instance.LoggingService.Logger.LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "DeltaSql.log");
            }
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
