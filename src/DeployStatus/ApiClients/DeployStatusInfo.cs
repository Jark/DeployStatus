using System.Collections.Generic;

namespace DeployStatus.ApiClients
{
    public class DeployStatusInfo
    {
        public OctopusEnvironmentInfo Environment { get; }
        public List<TeamCityBuildInfo> BuildInfo { get; }
        public IEnumerable<TrelloCardInfo> TrelloCards { get; }
        public string BranchName { get; }

        public DeployStatusInfo(OctopusEnvironmentInfo environment, List<TeamCityBuildInfo> buildInfo,
            IEnumerable<TrelloCardInfo> trelloCards, string branchName)
        {
            Environment = environment;
            BuildInfo = buildInfo;
            TrelloCards = trelloCards;
            BranchName = branchName;
        }
    }
}