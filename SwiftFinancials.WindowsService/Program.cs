using System;
using System.ServiceProcess;
using System.Threading;

namespace SwiftFinancials.WindowsService
{
    static class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--check-startup")
            {
                using (var startupCheckService = new MainService())
                {
                    try { startupCheckService.ValidateStartup(); return 0; }
                    catch (Exception ex) { StartupDiagnostics.Write("Startup check failed.", ex); return 1; }
                    finally { startupCheckService.CloseDiagnostics(); }
                }
            }
            if (args.Length != 0)
            {
                Console.Error.WriteLine("Usage: SwiftFinancials.WindowsService.exe [--check-startup]");
                return 2;

            }
#if (!DEBUG)
            ServiceBase.Run(new ServiceBase[] { new MainService() });
#else
            var service = new MainService();
            service.StartDebugging(args);
            Thread.Sleep(Timeout.Infinite);
#endif
            return 0;
        }
    }
}
