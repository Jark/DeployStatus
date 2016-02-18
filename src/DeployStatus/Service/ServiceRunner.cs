using System;
using System.ServiceProcess;
using log4net;

namespace DeployStatus.Service
{
    public class ServiceRunner : ServiceBase
    {
        private readonly ILog log;
        private readonly IService service;

        public ServiceRunner(IService service)
        {
            log = LogManager.GetLogger(typeof(ServiceRunner));
            this.service = service;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                service.Start();
            }
            catch (Exception ex)
            {
                log.Error("Error during OnStart.", ex);
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                service.Stop();
            }
            catch (Exception ex)
            {
                log.Error("Unable to dispose server.", ex);
                throw;
            }
        }
    }
}