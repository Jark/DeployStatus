using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace DeployStatus.ApiClients
{
    public class OctopusEnvironmentInfo
    {
        public string Id { get; }
        public string Name { get; }
        public IReadOnlyCollection<OctopusMachineInfo> Machines { get; }
        public DateTimeOffset? StartTime { get; }
        public string Duration { get; }
        public string ErrorMessage { get; }
        public TaskState State { get; }
        public string ReleaseVersion { get; }
        public string ReleaseNotes { get; }
        public string DisplayName { get; }
        public string Username { get; }
        public string AbsoluteDeployLink { get; }

        public OctopusEnvironmentInfo(
            string id, string name, IReadOnlyCollection<OctopusMachineInfo> machines, DateTimeOffset? startTime, string duration,
            string errorMessage, TaskState state, string releaseVersion, string releaseNotes, string displayName,
            string username, string absoluteDeployLink)
        {
            Id = id;
            Name = name;
            Machines = machines;
            StartTime = startTime;
            Duration = duration;
            ErrorMessage = errorMessage;
            State = state;
            ReleaseVersion = releaseVersion;
            ReleaseNotes = releaseNotes;
            DisplayName = displayName;
            Username = username;
            AbsoluteDeployLink = absoluteDeployLink;
        }
    }
}