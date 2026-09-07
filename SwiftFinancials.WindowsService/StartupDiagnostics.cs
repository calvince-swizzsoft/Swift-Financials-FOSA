using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace SwiftFinancials.WindowsService
{
    // Must remain independent of Unity, Serilog, and application configuration parsing.
    internal static class StartupDiagnostics
    {
        internal static void Write(string message, Exception exception = null)
        {
            var text = new StringBuilder(DateTimeOffset.Now.ToString("O") + " " + message);
            if (exception != null)
            {
                text.AppendLine().Append(exception);
                for (var current = exception; current != null; current = current.InnerException)
                {
                    var load = current as ReflectionTypeLoadException;
                    if (load != null)
                        foreach (var error in load.LoaderExceptions) text.AppendLine().Append(error);
                }
            }
            var value = text.ToString();
            try { if (exception == null) Console.Out.WriteLine(value); else Console.Error.WriteLine(value); } catch { }
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SwiftFinancials", "Logs");
            try
            {
                Directory.CreateDirectory(directory);
                File.AppendAllText(Path.Combine(directory, "startup-" + DateTime.Today.ToString("yyyyMMdd") + ".log"), value + Environment.NewLine);
            }
            catch
            {
                try { File.AppendAllText(Path.Combine(Path.GetTempPath(), "SwiftFinancials-startup.log"), value + Environment.NewLine); } catch { }
            }
            if (exception != null)
                try { EventLog.WriteEntry("SwiftFinancialsService", value.Length > 30000 ? value.Substring(0, 30000) : value, EventLogEntryType.Error); } catch { }
        }
    }
}
