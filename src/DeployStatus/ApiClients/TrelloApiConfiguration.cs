using System.Collections.Generic;

namespace DeployStatus.ApiClients
{
    public class TrelloApiConfiguration
    {
        public string Key { get; }
        public string Token { get; }
        public string BoardName { get; }
        public IEnumerable<string> FilterCardsFromColumns { get; }

        public TrelloApiConfiguration(string key, string token, string boardName, IEnumerable<string> filterCardsFromColumns)
        {
            Key = key;
            Token = token;
            BoardName = boardName;
            FilterCardsFromColumns = filterCardsFromColumns;
        }
    }
}