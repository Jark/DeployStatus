using System.Collections.Generic;

namespace DeployStatus.Configuration
{
    public class EmailNotificationConfiguration
    {
        public string BoardName { get; }
        public IEnumerable<string> MonitorCardsFromColumns { get; }
        public int ReportAfterDaysInColumn { get; }

        public EmailNotificationConfiguration(string boardName, IEnumerable<string> monitorCardsFromColumns, int reportAfterDaysInColumn)
        {
            BoardName = boardName;
            MonitorCardsFromColumns = monitorCardsFromColumns;
            ReportAfterDaysInColumn = reportAfterDaysInColumn;
        }
    }
}