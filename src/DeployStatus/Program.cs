using System;
using System.ServiceProcess;
using DeployStatus.EmailNotification;
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
            var emailNotificationService = new EmailNotificationService();
            if (args.Length > 0 && args[0] == "/c")
            {
                deployStatusService.Start();
                emailNotificationService.Start();
                Console.WriteLine("Service started, press any key to stop.");
                Console.ReadKey();
                deployStatusService.Stop();
                emailNotificationService.Stop();
            }
            else
            {
                ServiceBase.Run(new ServiceRunner(deployStatusService, emailNotificationService));
            }
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.ErrorFormat("Unhandled exception occurred {0}", e.ExceptionObject);
        }
    }
}