using System.Configuration;

namespace DeployStatus.Configuration
{
    public class TrelloEmailResolverSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("DomainToAppend", IsRequired = true)]
        public string DomainToAppend
        {
            get { return (string) this["DomainToAppend"]; }
            set { this["DomainToAppend"] = value; }
        }
    }
}