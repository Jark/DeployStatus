using System.Configuration;

namespace DeployStatus.Configuration
{
    public class DeploymentLinkingSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("BoardName", IsRequired = true)]
        public string BoardName
        {
            get { return (string)this["BoardName"]; }
            set { this["BoardName"] = value; }
        }

        [ConfigurationProperty("FilterCardsFromColumns", IsRequired = false)]
        public string FilterCardsFromColumns
        {
            get { return (string)this["FilterCardsFromColumns"]; }
            set { this["FilterCardsFromColumns"] = value; }
        }
    }
}