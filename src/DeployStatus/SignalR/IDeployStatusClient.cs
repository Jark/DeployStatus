namespace DeployStatus.SignalR
{
    public interface IDeployStatusClient
    {
        void DeploySystemStatusChanged(DeploySystemStatus systemStatus);
    }
}