using System;
using System.Collections.Generic;
using System.ServiceProcess;
using log4net;

namespace DeployStatus.Service
{
    public class ServiceRunner : ServiceBase
    {
        private readonly ILog log;
        private readonly IEnumerable<IService> services;

        public ServiceRunner(params IService[] services)
        {
            log = LogManager.GetLogger(typeof(ServiceRunner));
            this.services = services;
        }

        protected override void OnStart(string[] args)
        {
            foreach (var service in services)
            {
                try
                {
                    service.Start();
                }
                catch (Exception ex)
                {
                    log.Error($"Error during OnStart for service: {service}.", ex);
                    throw;
                }
            }           
        }

        protected override void OnStop()
        {
            foreach (var service in services)
            {
                try
                {
                    service.Stop();
                }
                catch (Exception ex)
                {
                    log.Error($"Unable to dispose service {service}.", ex);
                    throw;
                }
            }           
        }
    }
}