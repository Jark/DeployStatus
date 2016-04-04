using DeployStatus.ApiClients;

namespace DeployStatus.Configuration
{
    public class DeployStatusConfiguration
    {
        public string Name { get; }
        public string WebAppUrl { get; }
        public bool PerformEmailNotificationsCheckOnStartup { get; }
        public TrelloApiConfiguration Trello { get; }
        public OctopusApiConfiguration Octopus { get; }
        public TeamCityApiConfiguration TeamCity { get; }
        public IDeployUserResolver DeployUserResolver { get; }

        public DeployStatusConfiguration(string name, string webAppUrl, bool performEmailNotificationsCheckOnStartup, TrelloApiConfiguration trello, OctopusApiConfiguration octopus, TeamCityApiConfiguration teamCity, IDeployUserResolver deployUserResolver = null)
        {
            Name = name;
            WebAppUrl = webAppUrl;
            PerformEmailNotificationsCheckOnStartup = performEmailNotificationsCheckOnStartup;
            Trello = trello;
            Octopus = octopus;
            TeamCity = teamCity;
            DeployUserResolver = deployUserResolver ?? new SimpleDeployUserResolver();
        }
    }
}