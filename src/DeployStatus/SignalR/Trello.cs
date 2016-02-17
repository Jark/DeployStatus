namespace DeployStatus.SignalR
{
    public class Trello
    {
        public string Id { get; }
        public string Name { get; }
        public string Link { get; }

        public Trello(string id, string name, string link)
        {
            Id = id;
            Name = name;
            Link = link;
        }
    }
}