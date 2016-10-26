using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace DeployStatus.ApiClients
{
    internal class TeamCityClient
    {
        private readonly RestClient restClient;

        public TeamCityClient(TeamCityApiConfiguration teamCityApiConfiguration)
        {
            restClient = new RestClient(teamCityApiConfiguration.ServerUri);

            restClient.ReadWriteTimeout = 60000;
            restClient.Timeout = 60000;
        }

        public async Task<IEnumerable<TeamCityBuildInfo>> GetBuildsContaining(Version version)
        {
            var restRequest = new RestRequest("guestAuth/app/rest/builds/");
            restRequest.AddParameter("locator", $"project:SwanSystem,branch:default:any,number:{version.Major}.{version.Minor}.{version.Build}", ParameterType.QueryString);

            var result = (await restClient.ExecuteGetTaskAsync<TeamCityBuildResults>(restRequest)).Data;
            if (result.Build == null)
                return Enumerable.Empty<TeamCityBuildInfo>();

            var builds = new List<TeamCityBuildInfo>();
            foreach (var teamCityBuildResult in result.Build)
            {
                var detailedBuildResultTask = (await restClient.ExecuteGetTaskAsync<TeamCityDetailedBuild>(new RestRequest(teamCityBuildResult.Href)));
                var detailedBuildResult = detailedBuildResultTask.Data;

                builds.Add(new TeamCityBuildInfo(teamCityBuildResult.Id, teamCityBuildResult.BuildTypeId, teamCityBuildResult.BranchName,
                    teamCityBuildResult.WebUrl, teamCityBuildResult.Status, GetDate(detailedBuildResult.Triggered),
                    GetUsers(detailedBuildResult.Triggered), GetLastChanges(detailedBuildResult.LastChanges)));
            }
            return builds;
        }

        private static IEnumerable<ChangeInfo> GetLastChanges(TeamCityLastChanges lastChanges)
        {
            if (lastChanges?.Change == null)
                return Enumerable.Empty<ChangeInfo>();

            return lastChanges.Change.Select(x => new ChangeInfo(x.Username, x.Version));
        }

        private static IEnumerable<string> GetUsers(TeamCityTriggeredBy triggeredBy)
        {
            if (triggeredBy?.User == null)
                return Enumerable.Empty<string>();

            return triggeredBy.User.Select(GetUsername).ToList();
        }

        private static DateTime GetDate(TeamCityTriggeredBy triggeredBy)
        {
            if (triggeredBy == null)
                return default(DateTime);

            // todo: parse time zones which don't use whole hours
            return DateTime.ParseExact(triggeredBy.Date, "yyyyMMddTHHmmsszz00", CultureInfo.InvariantCulture);
        }

        private static string GetUsername(TeamCityUser user)
        {
            if (!string.IsNullOrWhiteSpace(user.Name))
                return user.Name;

            return user.Username;
        }

        private class TeamCityBuildResults
        {
            public int Count { get; private set; }
            public string Href { get; private set; }
            public List<TeamCityBuildResult> Build { get; private set; }
        }

        private class TeamCityDetailedBuild
        {
            public int Id { get; private set; }
            public TeamCityTriggeredBy Triggered { get; private set; }
            public TeamCityLastChanges LastChanges { get; private set; }
        }

        private class TeamCityChange
        {
            public int Id { get; private set; }
            public string Version { get; private set; }
            public string Username { get; private set; }
            public string Date { get; private set; }
            public string Href { get; private set; }
            public string WebUrl { get; private set; }
        }
        private class TeamCityLastChanges
        {
            public int Count { get; private set; }
            public List<TeamCityChange> Change { get; private set; }
        }

        private class TeamCityTriggeredBy
        {
            public string Type { get; private set; }
            public string Date { get; private set; }
            public List<TeamCityUser> User { get; private set; }
        }
        private class TeamCityUser
        {
            public int Id { get; private set; }
            public string Username { get; private set; }
            public string Href { get; private set; }
            public string Name { get; private set; }
        }

        private class TeamCityBuildResult
        {
            public int Id { get; private set; }
            public string BuildTypeId { get; private set; }
            public string Number { get; private set; }
            public string Status { get; private set; }
            public string State { get; private set; }
            public string BranchName { get; private set; }
            public string Href { get; private set; }
            public string WebUrl { get; private set; }
        }
    }
}