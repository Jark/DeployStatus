using System.Configuration;

namespace DeployStatus.Configuration
{
    public class EmailNotificationSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("BoardName", IsRequired = true)]
        public string BoardName
        {
            get { return (string)this["BoardName"]; }
            set { this["BoardName"] = value; }
        }

        [ConfigurationProperty("MonitorCardsFromColumns", IsRequired = false)]
        public string MonitorCardsFromColumns
        {
            get { return (string)this["MonitorCardsFromColumns"]; }
            set { this["MonitorCardsFromColumns"] = value; }
        }

        [ConfigurationProperty("ReportAfterDaysInColumn", IsRequired = false)]
        public int ReportAfterDaysInColumn
        {
            get { return (int)this["ReportAfterDaysInColumn"]; }
            set { this["ReportAfterDaysInColumn"] = value; }
        }
    }
}