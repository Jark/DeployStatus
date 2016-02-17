namespace DeployStatus.ApiClients
{
    public class TeamCityApiConfiguration
    {
        public string ServerUri { get; }

        public TeamCityApiConfiguration(string serverUri)
        {
            ServerUri = serverUri;
        }
    }
}