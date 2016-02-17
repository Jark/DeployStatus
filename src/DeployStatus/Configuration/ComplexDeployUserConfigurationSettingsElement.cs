using System.Configuration;

namespace DeployStatus.Configuration
{
    public class ComplexDeployUserConfigurationSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("OctopusDeployUsersToIgnore", IsRequired = false)]
        public string OctopusDeployUsersToIgnore
        {
            get { return (string)this["OctopusDeployUsersToIgnore"]; }
            set { this["OctopusDeployUsersToIgnore"] = value; }
        }

        [ConfigurationProperty("TeamCityBuildTypesToGetUsersFrom", IsRequired = false)]
        public string TeamCityBuildTypesToGetUsersFrom
        {
            get { return (string)this["TeamCityBuildTypesToGetUsersFrom"]; }
            set { this["TeamCityBuildTypesToGetUsersFrom"] = value; }
        }
    }
}