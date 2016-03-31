using DeployStatus.Configuration;

namespace DeployStatus.ApiClients
{
    public class TrelloApiConfiguration
    {
        public TrelloEmailResolver EmailResolver { get; }
        public TrelloAuthentication Authentication { get; }
        public DeploymentLinkingConfiguration DeploymentLinkingConfiguration { get; }
        public EmailNotificationConfiguration EmailNotificationConfiguration { get; }

        public TrelloApiConfiguration(TrelloEmailResolver emailResolver, TrelloAuthentication key, DeploymentLinkingConfiguration deploymentLinkingConfiguration,
            EmailNotificationConfiguration emailNotificationConfiguration)
        {
            EmailResolver = emailResolver;
            Authentication = key;
            DeploymentLinkingConfiguration = deploymentLinkingConfiguration;
            EmailNotificationConfiguration = emailNotificationConfiguration;
        }
    }
}