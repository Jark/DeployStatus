using System.Collections.Generic;

namespace DeployStatus.ApiClients
{
    public class TrelloCardInfo
    {
        public string Id { get; }
        public string Name { get;  }
        public string Url { get; }
        public IEnumerable<string> Members { get; }

        public TrelloCardInfo(string id, string name, string url, IEnumerable<string> members)
        {
            Id = id;
            Name = name;
            Url = url;
            Members = members;
        }
    }
}