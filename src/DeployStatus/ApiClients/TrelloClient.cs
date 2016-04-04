using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeployStatus.Configuration;
using log4net;
using RestSharp;
using RestSharp.Authenticators;

namespace DeployStatus.ApiClients
{
    internal class TrelloClient
    {
        private readonly RestClient restClient;
        private readonly TrelloEmailResolver emailResolver;
        private readonly string deploymentLinkingSearchTemplate;
        private readonly string emailNotificationSearchString;
        private readonly int reportAfterDaysInColumn;
        private readonly string labelSearchTemplate;
        private readonly ILog log;

        public TrelloClient(TrelloApiConfiguration configuration)
        {
            restClient = new RestClient("https://api.trello.com/1/");
            restClient.Authenticator = GetAuthenticator(configuration.Authentication);

            emailResolver = configuration.EmailResolver;
            deploymentLinkingSearchTemplate = GetDeploymentLinkingSearchTemplate(configuration.DeploymentLinkingConfiguration);
            labelSearchTemplate = GetLabelSearchTemplate(configuration.DeploymentLinkingConfiguration);
            emailNotificationSearchString = GetEmailNotificationSearchString(configuration.EmailNotificationConfiguration);
            reportAfterDaysInColumn = configuration.EmailNotificationConfiguration.ReportAfterDaysInColumn;

            log = LogManager.GetLogger(typeof (TrelloClient));
        }

        private static SimpleAuthenticator GetAuthenticator(TrelloAuthentication trelloAuthentication)
        {
            return new SimpleAuthenticator("key", trelloAuthentication.Key, "token", trelloAuthentication.Token);
        }

        private static string GetDeploymentLinkingSearchTemplate(DeploymentLinkingConfiguration deploymentLinkingConfiguration)
        {
            return $"board:\"{deploymentLinkingConfiguration.BoardName}\" is:open {string.Join(" ", deploymentLinkingConfiguration.FilterCardsFromColumns.Select(x => $"-list:{x}"))}" + " {0}";
        }

        private static string GetLabelSearchTemplate(DeploymentLinkingConfiguration deploymentLinkingConfiguration)
        {
            return $"board:\"{deploymentLinkingConfiguration.BoardName}\" is:open {string.Join(" ", deploymentLinkingConfiguration.FilterCardsFromColumns.Select(x => $"-list:{x}"))}" + " label:\"{0}\"";
        }

        private string GetEmailNotificationSearchString(EmailNotificationConfiguration emailNotificationConfiguration)
        {
            return $"board:\"{emailNotificationConfiguration.BoardName}\" is:open {string.Join(" ", emailNotificationConfiguration.MonitorCardsFromColumns.Select(x => $"list:{x}"))}";
        }

        public async Task<IEnumerable<TrelloCardInfo>> GetCardsThatAreInactive()
        {
            var cardsInMonitorColumns = await GetCardsContaining(emailNotificationSearchString);
            return cardsInMonitorColumns.Where(x => x.LastActivity <= DateTime.UtcNow.AddDays(-reportAfterDaysInColumn));
        }

        public async Task<IEnumerable<TrelloCardInfo>> GetCardsLinkedToBranch(string searchString)
        {
            var deploymentLinkingSearchString = string.Format(deploymentLinkingSearchTemplate, searchString);
            return await GetCardsContaining(deploymentLinkingSearchString);
        }

        public async Task<IEnumerable<TrelloCardInfo>> GetCardsLinkedToLabel(string label)
        {
            var deploymentLinkingSearchString = string.Format(labelSearchTemplate, label);
            return await GetCardsContaining(deploymentLinkingSearchString);
        }

        private async Task<IEnumerable<TrelloCardInfo>> GetCardsContaining(string searchString)
        {
            var searchResult = await ExecuteSearchCards(searchString);
            var trelloCardInfos = searchResult.Cards.Select(x =>
            {
                var members = x.Members.Select(y => GetTrelloMemberInfo(y.FullName)).ToList();
                return new TrelloCardInfo(x.Id, x.Name, x.Url, members, GetLastActivity(x), x.List.Name);
            });

            return trelloCardInfos.ToList();
        }

        private static DateTime GetLastActivity(Card x)
        {
            return DateTime.Parse(x.DateLastActivity);
        }

        private TrelloMemberInfo GetTrelloMemberInfo(string fullName)
        {
            return new TrelloMemberInfo(fullName, emailResolver.GetEmail(fullName));
        }

        private async Task<SearchResult> ExecuteSearchCards(string searchString)
        {
            var restRequest = new RestRequest("search/");
            restRequest.AddParameter("modelTypes", "cards,members");
            restRequest.AddParameter("query", searchString);
            restRequest.AddParameter("cards_limit", 100); // cap at 100 cards for now
            restRequest.AddParameter("card_fields", "url,name,dateLastActivity");
            restRequest.AddParameter("card_members", "true");
            restRequest.AddParameter("card_list", "true");

            var result = await restClient.ExecuteGetTaskAsync<SearchResult>(restRequest);
            if (result.ResponseStatus != ResponseStatus.Completed)
            {
                log.Warn($"Did not receive a valid response from server using searchstring: {searchString}, received instead: {result.Content}. Retrying.");
                result = await restClient.ExecuteGetTaskAsync<SearchResult>(restRequest);
            }
            return result.Data;
        }

        private class SearchResult
        {
            public List<Card> Cards { get; private set; }
        }

        private class Card
        {
            public string Id { get; private set; }
            public List<Member> Members { get; private set; }
            public string Name { get; private set; }
            public string Url { get; private set; }
            public string DateLastActivity { get; private set; }
            public List List { get; private set; }
        }

        private class Member
        {
            public string Id { get; private set; }
            public string Username { get; private set; }
            public string FullName { get; private set; }
            public string Initials { get; private set; }
            public string AvatarHash { get; private set; }
        }

        private class List
        {
            public string Id { get; private set; }
            public string Name { get; private set; }
            public bool Closed { get; private set; }
            public string IdBoard { get; private set; }
            public int Pos { get; private set; }
            public bool Subscribed { get; private set; }
        }
    }
}