using System.Configuration;

namespace DeployStatus.Configuration
{
    public class TrelloSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("Key", IsRequired = true)]
        public string Key
        {
            get { return (string)this["Key"]; }
            set { this["Key"] = value; }
        }

        [ConfigurationProperty("Token", IsRequired = true)]
        public string Token
        {
            get { return (string)this["Token"]; }
            set { this["Token"] = value; }
        }

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