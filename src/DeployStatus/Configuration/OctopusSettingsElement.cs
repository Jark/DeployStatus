using System.Configuration;

namespace DeployStatus.Configuration
{
    public class OctopusSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("ServerUri", IsRequired = true)]
        public string ServerUri
        {
            get { return (string)this["ServerUri"]; }
            set { this["ServerUri"] = value; }
        }

        [ConfigurationProperty("ApiKey", IsRequired = true)]
        public string ApiKey
        {
            get { return (string)this["ApiKey"]; }
            set { this["ApiKey"] = value; }
        }
    }
}