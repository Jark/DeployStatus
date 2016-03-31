using DeployStatus.Configuration;

namespace DeployStatus.ApiClients
{
    public class TrelloApiConfiguration
    {
        public TrelloAuthentication Authentication { get; }
        public DeploymentLinkingConfiguration DeploymentLinkingConfiguration { get; }
        public EmailNotificationConfiguration EmailNotificationConfiguration { get; }

        public TrelloApiConfiguration(TrelloAuthentication key, DeploymentLinkingConfiguration deploymentLinkingConfiguration,
            EmailNotificationConfiguration emailNotificationConfiguration)
        {
            Authentication = key;
            DeploymentLinkingConfiguration = deploymentLinkingConfiguration;
            EmailNotificationConfiguration = emailNotificationConfiguration;
        }
    }
}