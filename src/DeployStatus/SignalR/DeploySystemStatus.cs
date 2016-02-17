using System;
using System.Collections.Generic;

namespace DeployStatus.SignalR
{
    public class DeploySystemStatus
    {
        public string Name { get; }
        public DateTime LastUpdated { get; }
        public IEnumerable<Environment> Environments { get; }

        public DeploySystemStatus(string name, DateTime lastUpdated, IEnumerable<Environment> environments)
        {
            Name = name;
            LastUpdated = lastUpdated;
            Environments = environments;
        }
    }
}