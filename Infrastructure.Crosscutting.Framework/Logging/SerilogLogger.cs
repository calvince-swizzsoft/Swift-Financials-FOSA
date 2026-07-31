using Serilog;
using System;
using System.ComponentModel.Composition;

namespace Infrastructure.Crosscutting.Framework.Logging
{
    [Export(typeof(ILogger))]
    public class SerilogLogger : ILogger
    {
        public SerilogLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();
        }

        public void Debug(object item)
        {
            Log.Debug($"{item}");
        }

        public void Debug(string message, params object[] args)
        {
            Log.Debug(message, args);
        }

        public void Debug(string message, Exception exception, params object[] args)
        {
            Log.Debug(exception, message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            Log.Fatal(message, args);
        }

        public void Fatal(string message, Exception exception, params object[] args)
        {
            Log.Fatal(exception, message, args);
        }

        public void LogError(string message, params object[] args)
        {
            Log.Error(message, args);
        }

        public void LogError(string message, Exception exception, params object[] args)
        {
            Log.Error(exception, message, args);
        }

        public void LogInfo(string message, params object[] args)
        {
            Log.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            Log.Warning(message, args);
        }

        public void CloseAndFlush()
        {
            Log.CloseAndFlush();
        }
    }
}
