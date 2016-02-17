using DeployStatus.ApiClients;

namespace DeployStatus.Configuration
{
    public interface IDeployUserResolver
    {
        string GetDeployer(DeployStatusInfo deployStatusInfo);
    }
}