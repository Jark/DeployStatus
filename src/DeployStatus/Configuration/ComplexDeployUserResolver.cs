using System;
using System.Collections.Generic;
using System.Linq;
using DeployStatus.ApiClients;

namespace DeployStatus.Configuration
{
    public class ComplexDeployUserResolver : IDeployUserResolver
    {
        private readonly IEnumerable<string> octopusDeployUsersToIgnore;
        private readonly IEnumerable<string> teamCityBuildTypesToGetUsersFrom;

        public ComplexDeployUserResolver(IEnumerable<string> octopusDeployUsersToIgnore,
            IEnumerable<string> teamCityBuildTypesToGetUsersFrom)
        {
            this.octopusDeployUsersToIgnore = octopusDeployUsersToIgnore;
            this.teamCityBuildTypesToGetUsersFrom = teamCityBuildTypesToGetUsersFrom;
        }

        public string GetDeployer(DeployStatusInfo deployStatusInfo)
        {
            string deployerName;
            if (TryGetFromOctopus(deployStatusInfo.Environment, out deployerName) ||
                TryGetFromTeamCity(deployStatusInfo.BuildInfo, out deployerName) ||
                TryGetFromTrello(deployStatusInfo.BranchRelatedTrellos, out deployerName) ||
                TryGetFromTrello(deployStatusInfo.EnvironmentTaggedTrellos, out deployerName))
                return deployerName;

            return deployStatusInfo.Environment.DisplayName;
        }

        private bool TryGetFromOctopus(OctopusEnvironmentInfo environment, out string deployerName)
        {
            if (!octopusDeployUsersToIgnore.Any(x => x.Equals(environment.DisplayName, StringComparison.OrdinalIgnoreCase)))
            {
                deployerName = environment.DisplayName;
                return true;
            }

            deployerName = null;
            return false;
        }

        private bool TryGetFromTeamCity(List<TeamCityBuildInfo> buildInfo, out string deployerName)
        {
            var limitedBuildInfo = buildInfo.Where(x => teamCityBuildTypesToGetUsersFrom.Contains(x.BuildTypeId, StringComparer.OrdinalIgnoreCase)).ToList();

            var firstBuildUserThatMakesSense =
                limitedBuildInfo.Where(x => x.Users != null).SelectMany(x => x.Users).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(firstBuildUserThatMakesSense))
            {
                deployerName = firstBuildUserThatMakesSense;
                return true;
            }

            var firstCommitUserThatMakesSense = limitedBuildInfo
                .Where(x => x.Changes != null)
                .SelectMany(x => x.Changes)
                .Select(x => x.Username).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(firstCommitUserThatMakesSense))
            {
                deployerName = firstCommitUserThatMakesSense;
                return true;
            }

            deployerName = null;
            return false;
        }

        private static bool TryGetFromTrello(IEnumerable<TrelloCardInfo> trelloCards, out string deployerName)
        {
            var firstMemberThatMakesSense = trelloCards.SelectMany(x => x.Members).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(firstMemberThatMakesSense?.Name))
            {
                deployerName = firstMemberThatMakesSense.Name;
                return true;
            }

            deployerName = null;
            return false;
        }
    }
}