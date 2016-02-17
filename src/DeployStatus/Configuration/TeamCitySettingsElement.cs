using System.Configuration;

namespace DeployStatus.Configuration
{
    public class TeamCitySettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("ServerUri", IsRequired = true)]
        public string ServerUri
        {
            get { return (string)this["ServerUri"]; }
            set { this["ServerUri"] = value; }
        }
    }
}