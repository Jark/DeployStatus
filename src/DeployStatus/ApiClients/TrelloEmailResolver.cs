namespace DeployStatus.ApiClients
{
    // Trello does not return email addresses for other users, so we try and guess the email address    
    public class TrelloEmailResolver
    {
        private readonly string domainToAppend;

        public TrelloEmailResolver(string domainToAppend)
        {
            this.domainToAppend = domainToAppend;
        }

        public string GetEmail(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            return $"{fullName.Replace(' ', '.')}@{domainToAppend}";
        }
    }
}