using System.Collections.Generic;

namespace DeployStatus.ApiClients
{
    public class DeployStatusInfo
    {
        public OctopusEnvironmentInfo Environment { get; }
        public List<TeamCityBuildInfo> BuildInfo { get; }
        public IEnumerable<TrelloCardInfo> BranchRelatedTrellos { get; }
        public IEnumerable<TrelloCardInfo> EnvironmentTaggedTrellos { get; }    
        public string BranchName { get; }

        public DeployStatusInfo(OctopusEnvironmentInfo environment, List<TeamCityBuildInfo> buildInfo, IEnumerable<TrelloCardInfo> branchRelatedTrellos, IEnumerable<TrelloCardInfo> environmentTaggedCards, string branchName)
        {
            Environment = environment;
            BuildInfo = buildInfo;
            BranchRelatedTrellos = branchRelatedTrellos;
            EnvironmentTaggedTrellos = environmentTaggedCards;
            BranchName = branchName;
        }

    }
}