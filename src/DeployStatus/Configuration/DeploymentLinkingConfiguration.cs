using System.Collections.Generic;

namespace DeployStatus.Configuration
{
    public class DeploymentLinkingConfiguration
    {
        public string BoardName { get; }
        public IEnumerable<string> FilterCardsFromColumns { get; }

        public DeploymentLinkingConfiguration(string boardName, IEnumerable<string> filterCardsFromColumns)
        {
            BoardName = boardName;
            FilterCardsFromColumns = filterCardsFromColumns;
        }
    }
}