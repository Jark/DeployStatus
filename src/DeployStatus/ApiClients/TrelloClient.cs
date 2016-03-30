using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace DeployStatus.ApiClients
{
    internal class TrelloClient
    {
        private readonly RestClient restClient;
        private readonly string searchTemplate;

        public TrelloClient(TrelloApiConfiguration configuration)
        {
            restClient = new RestClient("https://api.trello.com/1/");
            restClient.Authenticator = new SimpleAuthenticator("key", configuration.Key, "token", configuration.Token);
            searchTemplate = $"board:\"{configuration.BoardName}\" is:open {string.Join(" ", configuration.FilterCardsFromColumns.Select(x => $"-list:{x}"))}" + " {0}";
        }

        public async Task<IEnumerable<TrelloCardInfo>> GetCardsContaining(string searchString)
        {
            var searchResult = await ExecuteSearchCards(searchString);
            var membersFromServer = await Task.WhenAll(searchResult.Cards
                .SelectMany(x => x.IdMembers)
                .Distinct()
                .Select(async x => await ExecuteGetMember(x)));

            var membersById = membersFromServer.ToDictionary(x => x.Id, x => x.FullName);
            var trelloCardInfos = searchResult.Cards.Select(x =>
            {
                var members = x.IdMembers.Select(y => membersById[y]).ToList();
                return new TrelloCardInfo(x.Id, x.Name, x.Url, members);
            });

            return trelloCardInfos.ToList();
        }

        private async Task<SearchResult> ExecuteSearchCards(string searchString)
        {
            var restRequest = new RestRequest("search/");
            restRequest.AddParameter("modelTypes", "cards,members");
            restRequest.AddParameter("query", string.Format(searchTemplate, searchString));
            restRequest.AddParameter("cards_limit", 10); // cap at 10 cards for now
            restRequest.AddParameter("card_fields", "idMembers,url,name");

            var result = await restClient.ExecuteGetTaskAsync<SearchResult>(restRequest);
            return result.Data;
        }

        private async Task<Member> ExecuteGetMember(string memberId)
        {
            var restRequest = new RestRequest("members/{id}");
            restRequest.AddUrlSegment("id", memberId);

            var result = await restClient.ExecuteGetTaskAsync<Member>(restRequest);
            return result.Data;
        }

        private class SearchResult
        {
            public List<Card> Cards { get; private set; }
        }

        private class Card
        {
            public string Id { get; private set; }
            public List<string> IdMembers { get; private set; }
            public string Name { get; private set; }
            public string Url { get; private set; }
        }

        private class Member
        {
            public string Id { get; private set; }
            public string FullName { get; private set; }
        }
    }
}