namespace DeployStatus.ApiClients
{
    public class ChangeInfo
    {
        public string Username { get; }
        public string GitSha { get; }

        public ChangeInfo(string username, string gitSha)
        {
            Username = username;
            GitSha = gitSha;
        }        
    }
}