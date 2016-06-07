using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace DeployStatus.SignalR
{
    public class Environment
    {
        public string Id { get; }
        public string Name { get; }
        public string Version { get; }
        public DateTimeOffset Started { get; }
        public TaskState State { get; }
        public string Branch { get; }
        public string OctopusDeployLink { get; }
        public string DeployedBy { get; }
        public IEnumerable<Trello> BranchRelatedTrellos { get; }
        public IEnumerable<Trello> EnvironmentTaggedTrellos { get; }
        public IEnumerable<TeamCityBuild> Builds { get; }
        public bool IsDisabled { get; }

        public Environment(
            string id, string name, string version, DateTimeOffset started, TaskState state, 
            string branch, string octopusDeployLink, string deployedBy, 
            bool isDisabled,
            IEnumerable<Trello> branchRelatedTrellos, 
            IEnumerable<Trello> environmentTaggedTrellos,
            IEnumerable<TeamCityBuild> builds)
        {
            Id = id;
            Name = name;
            Version = version;
            Started = started;
            State = state;
            Branch = branch;
            OctopusDeployLink = octopusDeployLink;
            DeployedBy = deployedBy;
            IsDisabled = isDisabled;
            BranchRelatedTrellos = branchRelatedTrellos;
            EnvironmentTaggedTrellos = environmentTaggedTrellos;
            Builds = builds;
        }
    }
}