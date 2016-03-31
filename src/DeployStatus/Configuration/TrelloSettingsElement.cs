using System.Configuration;

namespace DeployStatus.Configuration
{
    public class TrelloSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("EmailResolver", IsRequired = true)]
        public TrelloEmailResolverSettingsElement EmailResolver
        {
            get { return (TrelloEmailResolverSettingsElement) this["EmailResolver"]; }
            set { this["EmailResolver"] = value; }
        }

        [ConfigurationProperty("Authentication", IsRequired = true)]
        public TrelloAuthenticationSettingsElement Authentication
        {
            get { return (TrelloAuthenticationSettingsElement) this["Authentication"]; }
            set { this["Authentication"] = value; }
        }

        [ConfigurationProperty("DeploymentLinking", IsRequired = false)]
        public DeploymentLinkingSettingsElement DeploymentLinking
        {
            get { return (DeploymentLinkingSettingsElement) this["DeploymentLinking"]; }
            set { this["DeploymentLinking"] = value; }
        }

        [ConfigurationProperty("EmailNotification", IsRequired = false)]
        public EmailNotificationSettingsElement EmailNotification
        {
            get { return (EmailNotificationSettingsElement) this["EmailNotification"]; }
            set { this["EmailNotification"] = value; }
        }
    }
}