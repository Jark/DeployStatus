namespace DeployStatus.Configuration
{
    public class TrelloAuthentication
    {
        public string Key { get; }
        public string Token { get; }

        public TrelloAuthentication(string key, string token)
        {
            Key = key;
            Token = token;
        }
    }
}