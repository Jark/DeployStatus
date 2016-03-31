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
                settings.Octopus.AsOctopusApiConfiguration(),
                settings.TeamCity.AsTeamCityApiConfiguration(),
                settings.ComplexDeployUserConfiguration.AsDeployUserResolver());
        }

        public static TrelloApiConfiguration AsTrelloApiConfiguration(this TrelloSettingsElement trello)
        {
            return new TrelloApiConfiguration(
                trello.EmailResolver.AsEmailResolver(),
                trello.Authentication.AsAuthentication(),
                trello.DeploymentLinking.AsDeploymentLinkingConfiguration(), 
                trello.EmailNotification.AsEmailNotificationConfiguration());
        }

        private static TrelloEmailResolver AsEmailResolver(this TrelloEmailResolverSettingsElement emailResolverSettings)
        {
            return new TrelloEmailResolver(emailResolverSettings.DomainToAppend);
        }

        private static TrelloAuthentication AsAuthentication(this TrelloAuthenticationSettingsElement authentication)
        {
            return new TrelloAuthentication(authentication.Key, authentication.Token);
        }

        private static DeploymentLinkingConfiguration AsDeploymentLinkingConfiguration(this DeploymentLinkingSettingsElement linkingSettings)
        {
            return new DeploymentLinkingConfiguration(linkingSettings.BoardName, GetListFromCommaSeparatedSequence(linkingSettings.FilterCardsFromColumns));
        }

        private static EmailNotificationConfiguration AsEmailNotificationConfiguration(this EmailNotificationSettingsElement emailNotificationSettings)
        {
            return new EmailNotificationConfiguration(emailNotificationSettings.BoardName, GetListFromCommaSeparatedSequence(emailNotificationSettings.MonitorCardsFromColumns), emailNotificationSettings.ReportAfterDaysInColumn);
        }

        private static OctopusApiConfiguration AsOctopusApiConfiguration(this OctopusSettingsElement octopus)
        {
            return new OctopusApiConfiguration(octopus.ServerUri, octopus.ApiKey);
        }

        private static TeamCityApiConfiguration AsTeamCityApiConfiguration(this TeamCitySettingsElement teamCity)
        {
            return new TeamCityApiConfiguration(teamCity.ServerUri);
        }

        private static IDeployUserResolver AsDeployUserResolver(this ComplexDeployUserConfigurationSettingsElement complexDeployUserConfiguration)
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