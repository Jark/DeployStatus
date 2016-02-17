using DeployStatus.ApiClients;

namespace DeployStatus.Configuration
{
    public class SimpleDeployUserResolver : IDeployUserResolver
    {
        public string GetDeployer(DeployStatusInfo deployStatusInfo)
        {
            return deployStatusInfo.Environment.DisplayName;
        }
    }
}