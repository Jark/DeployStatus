using System.Collections.Generic;
using System.Linq;
using DeployStatus.ApiClients;

namespace DeployStatus.Configuration
{
    public static class DeployStatusSettingsExtensions
    {
        public static DeployStatusConfiguration AsDeployConfiguration(this DeployStatusSettingsSection settings)
        { 
            return new DeployStatusConfiguration(
                settings.Name,
                settings.WebAppUrl,
                settings.Trello.AsTrelloApiConfiguration(),
                GetOctopusApiConfiguration(settings.Octopus),
                GetTeamCityApiConfiguration(settings.TeamCity),
                GetDeployUserResolver(settings.ComplexDeployUserConfiguration));
        }

        private static TrelloApiConfiguration AsTrelloApiConfiguration(this TrelloSettingsElement trello)
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