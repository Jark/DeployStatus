using System;
using System.Collections.Generic;
using System.Linq;
using DeployStatus.ApiClients;
using DeployStatus.Configuration;
using DeployStatus.SignalR;
using Microsoft.Owin.Hosting;

namespace DeployStatus
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting api polling service...");

            var deployConfiguration = GetConfiguration(DeployStatusSettingsSection.Settings);

            DeployStatusState.Instance.Value.Start(deployConfiguration);

            var webAppUrl = deployConfiguration.WebAppUrl;
            var webApp = WebApp.Start<Startup>(webAppUrl);
            Console.WriteLine("Starting web app service on {0}...", webAppUrl);
            Console.WriteLine("Started, press any key to quit.");
            Console.ReadKey();

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