namespace DeployStatus.ApiClients
{
    public class TrelloMemberInfo
    {
        public string Name { get; }
        public string Email { get; }

        public TrelloMemberInfo(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}