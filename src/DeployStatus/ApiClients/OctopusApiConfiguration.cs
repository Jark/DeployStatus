namespace DeployStatus.ApiClients
{
    public class OctopusApiConfiguration
    {
        public string ServerUri { get; }
        public string ApiKey { get; }

        public OctopusApiConfiguration(string serverUri, string apiKey)
        {
            ServerUri = serverUri;
            ApiKey = apiKey;
        }
    }
}