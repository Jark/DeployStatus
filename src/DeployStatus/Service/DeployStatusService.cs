using System;
using System.Collections.Generic;
using System.Linq;
using DeployStatus.ApiClients;
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
            deployConfiguration = GetConfiguration(DeployStatusSettingsSection.Settings);
        }
        public void Start()
        {
            log.Info("Starting api polling service...");


            DeployStatusState.Instance.Value.Start(deployConfiguration);

            var webAppUrl = deployConfiguration.WebAppUrl;
            log.Info($"Starting web app service on {webAppUrl}...");
            webApp = WebApp.Start<Startup>(webAppUrl);
            log.Info("Started.");
        }

        public void Stop()
        {
            webApp.Dispose();
            DeployStatusState.Instance.Value.Stop();
        }

        private static DeployStatusConfiguration GetConfiguration(DeployStatusSettingsSection settings)
        {
            return new DeployStatusConfiguration(
                settings.Name,
                settings.WebAppUrl,
                GetTrelloApiConfiguration(settings.Trello),
                GetOctopusApiConfiguration(settings.Octopus),
                GetTeamCityApiConfiguration(settings.TeamCity),
                GetDeployUserResolver(settings.ComplexDeployUserConfiguration));
        }

        private static TrelloApiConfiguration GetTrelloApiConfiguration(TrelloSettingsElement trello)
        {
            return new TrelloApiConfiguration(trello.Key, trello.Token, trello.BoardName, GetListFromCommaSeparatedSequence(trello.FilterCardsFromColumns));
        }

        private static OctopusApiConfiguration GetOctopusApiConfiguration(OctopusSettingsElement octopus)
        {
            return new OctopusApiConfiguration(octopus.ServerUri, octopus.ApiKey);
        }

        private static TeamCityApiConfiguration GetTeamCityApiConfiguration(TeamCitySettingsElement teamCity)
        {
            return new TeamCityApiConfiguration(teamCity.ServerUri);
        }

        private static IDeployUserResolver GetDeployUserResolver(ComplexDeployUserConfigurationSettingsElement complexDeployUserConfiguration)
        {
            if (complexDeployUserConfiguration == null)
                return null;
            return new ComplexDeployUserResolver(
                GetListFromCommaSeparatedSequence(complexDeployUserConfiguration.OctopusDeployUsersToIgnore),
                GetListFromCommaSeparatedSequence(complexDeployUserConfiguration.TeamCityBuildTypesToGetUsersFrom));
        }

        private static IEnumerable<string> GetListFromCommaSeparatedSequence(string commaSeperatedItems)
        {
            if (commaSeperatedItems == null)
                return Enumerable.Empty<string>();

            return commaSeperatedItems.Split(',').Select(x => x.Trim());
        }
    }
}