using System.Configuration;

namespace DeployStatus.Configuration
{
    public class TrelloAuthenticationSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("Key", IsRequired = true)]
        public string Key
        {
            get { return (string) this["Key"]; }
            set { this["Key"] = value; }
        }

        [ConfigurationProperty("Token", IsRequired = true)]
        public string Token
        {
            get { return (string) this["Token"]; }
            set { this["Token"] = value; }
        }
    }
}