using System;
using System.Linq;
using DeployStatus.Configuration;
using DeployStatus.SignalR;
using log4net;
using Microsoft.Owin.Hosting;

namespace DeployStatus.Service
{
    public class DeployStatusService : IService
    {
        private IDisposable webApp;
        private readonly ILog log;
        private readonly DeployStatusConfiguration deployConfiguration;

        public DeployStatusService()
        {
            log = LogManager.GetLogger(typeof (DeployStatusService));
            deployConfiguration = DeployStatusSettingsSection.Settings.AsDeployConfiguration();
        }
        public void Start()
        {
            log.Info("Starting api polling service...");

            DeployStatusState.Instance.Value.Start(deployConfiguration);

            var webAppUrl = deployConfiguration.WebAppUrl;
            log.Info($"Starting web app service on {webAppUrl}...");

            var startOptions = new StartOptions();
            foreach (var url in webAppUrl.Split(','))
            {
                startOptions.Urls.Add(url);
            }

            webApp = WebApp.Start<Startup>(startOptions);
            log.Info("Started.");
        }

        public void Stop()
        {
            webApp.Dispose();
            DeployStatusState.Instance.Value.Stop();
        }
    }
}