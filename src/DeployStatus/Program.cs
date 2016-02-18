using System;
using System.ServiceProcess;
using DeployStatus.Service;
using log4net;

namespace DeployStatus
{
    internal class Program
    {
        private static ILog _log;

        static void Main(string[] args)
        {
            _log = LogManager.GetLogger(typeof(Program));
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
            var deployStatusService = new DeployStatusService();
            if (args.Length > 0 && args[0] == "/c")
            {
                deployStatusService.Start();
                Console.WriteLine("Service started, press any key to stop.");
                Console.ReadKey();
                deployStatusService.Stop();
            }
            else
            {
                ServiceBase.Run(new ServiceRunner(deployStatusService));
            }
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.ErrorFormat("Unhandled exception occurred {0}", e.ExceptionObject);
        }
    }
}