using System;
using System.Collections.Generic;

namespace DeployStatus.ApiClients
{
    public class TeamCityBuildInfo
    {
        public int Id { get; }
        public string BuildTypeId { get; }
        public string BranchName { get; }
        public string WebUrl { get; }
        public string Status { get; }
        public DateTime TriggeredAt { get; set; }
        public IEnumerable<string> Users { get; set; }
        public IEnumerable<ChangeInfo> Changes { get; set; }

        public TeamCityBuildInfo(int id, string buildTypeId, string branchName, string webUrl, string status,
            DateTime triggeredAt, IEnumerable<string> users, IEnumerable<ChangeInfo> changes)
        {
            Id = id;
            BuildTypeId = buildTypeId;
            BranchName = branchName;
            WebUrl = webUrl;
            Status = status;
            TriggeredAt = triggeredAt;
            Users = users;
            Changes = changes;
        }
    }
}