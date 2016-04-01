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

        protected bool Equals(TrelloMemberInfo other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Email, other.Email);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TrelloMemberInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name?.GetHashCode() ?? 0)*397) ^ (Email?.GetHashCode() ?? 0);
            }
        }
    }
}