using System.Configuration;

namespace DeployStatus.Configuration
{
    public class DeployStatusSettingsSection : ConfigurationSection
    {
        public static DeployStatusSettingsSection Settings { get; } =
            ConfigurationManager.GetSection("DeployStatus") as DeployStatusSettingsSection;

        [ConfigurationProperty("Name", IsRequired = false, DefaultValue = "Deploy Status")]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("WebAppUrl", IsRequired = false, DefaultValue = "http://+:5000")]
        public string WebAppUrl
        {
            get { return (string)this["WebAppUrl"]; }
            set { this["WebAppUrl"] = value; }
        }

        [ConfigurationProperty("PerformEmailNotificationsCheckOnStartup", IsRequired = false, DefaultValue = false)]
        public bool PerformEmailNotificationsCheckOnStartup
        {
            get { return (bool) this["PerformEmailNotificationsCheckOnStartup"]; }
            set { this["PerformEmailNotificationsCheckOnStartup"] = value; }
        }

        [ConfigurationProperty("Trello")]
        public TrelloSettingsElement Trello
        {
            get { return (TrelloSettingsElement) this["Trello"]; }
            set { this["Trello"] = value; }
        }

        [ConfigurationProperty("Octopus")]
        public OctopusSettingsElement Octopus
        {
            get { return (OctopusSettingsElement)this["Octopus"]; }
            set { this["Octopus"] = value; }
        }

        [ConfigurationProperty("TeamCity")]
        public TeamCitySettingsElement TeamCity
        {
            get { return (TeamCitySettingsElement)this["TeamCity"]; }
            set { this["TeamCity"] = value; }
        }

        [ConfigurationProperty("ComplexDeployUserConfiguration")]
        public ComplexDeployUserConfigurationSettingsElement ComplexDeployUserConfiguration
        {
            get { return (ComplexDeployUserConfigurationSettingsElement)this["ComplexDeployUserConfiguration"]; }
            set { this["ComplexDeployUserConfiguration"] = value; }
        }
    }
}