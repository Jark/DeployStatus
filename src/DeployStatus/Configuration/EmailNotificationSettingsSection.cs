using System.Configuration;

namespace DeployStatus.Configuration
{
    public class EmailNotificationSettingsSection : ConfigurationSection
    {
        public static DeployStatusSettingsSection Settings { get; } =
            ConfigurationManager.GetSection("EmailNotification") as DeployStatusSettingsSection;
    }
}