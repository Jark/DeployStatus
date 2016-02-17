using System;

namespace DeployStatus.SignalR
{
    public class TeamCityBuild
    {
        public int Id { get; }
        public string Name { get; }
        public string Link { get; }
        public string Status { get; }
        public DateTime TriggeredAt { get; }

        public TeamCityBuild(int id, string name, string link, string status, DateTime triggeredAt)
        {
            Id = id;
            Name = name;
            Link = link;
            Status = status;
            TriggeredAt = triggeredAt;
        }
    }
}