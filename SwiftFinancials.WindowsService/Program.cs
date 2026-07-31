using System.ServiceProcess;
using System.Threading;

namespace SwiftFinancials.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if (!DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new MainService() 
			};
            ServiceBase.Run(ServicesToRun);
#else
            MainService serviceBase = new MainService();
            serviceBase.StartDebugging(null);
            Thread.Sleep(Timeout.Infinite);
#endif
        }
    }
}
