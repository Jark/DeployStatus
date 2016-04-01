using System;
using System.Collections.Generic;

namespace DeployStatus.ApiClients
{
    public class TrelloCardInfo
    {
        public string Id { get; }
        public string Name { get;  }
        public string Url { get; }
        public IEnumerable<TrelloMemberInfo> Members { get; }
        public DateTime LastActivity { get; }
        public string ListName { get; }

        public TrelloCardInfo(string id, string name, string url, IEnumerable<TrelloMemberInfo> members,
            DateTime lastActivity, string listName)
        {
            Id = id;
            Name = name;
            Url = url;
            Members = members;
            LastActivity = lastActivity;
            ListName = listName;
        }

        protected bool Equals(TrelloCardInfo other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name) &&
                   string.Equals(ListName, other.ListName) && LastActivity.Equals(other.LastActivity) &&
                   Equals(Members, other.Members) && string.Equals(Url, other.Url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TrelloCardInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (ListName?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ LastActivity.GetHashCode();
                hashCode = (hashCode*397) ^ (Members?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (Url?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}