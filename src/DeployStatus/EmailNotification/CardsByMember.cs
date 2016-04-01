using System.Collections.Generic;
using System.Linq;
using DeployStatus.ApiClients;

namespace DeployStatus.EmailNotification
{
    internal class CardsByMember
    {
        public TrelloMemberInfo Member { get; }
        public IEnumerable<TrelloCardInfo> Cards { get; }

        public CardsByMember(TrelloMemberInfo key, IEnumerable<TrelloCardInfo> trelloCardInfos)
        {
            Member = key;
            Cards = trelloCardInfos;
        }

        public override string ToString()
        {
            return $"{Member.Name} ({Member.Email}), Cards={GetCardsAsString(Cards)}";
        }

        private static string GetCardsAsString(IEnumerable<TrelloCardInfo> cards)
        {
            return string.Join(", ", cards.Select(x => x.Name));
        }
    }
}