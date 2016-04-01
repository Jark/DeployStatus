using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeployStatus.Configuration;

namespace DeployStatus.ApiClients
{
    public class DeployStatusInfoClient
    {
        private readonly OctopusClient octopusClient;
        private readonly TrelloClient trelloClient;
        private readonly TeamCityClient teamCityClient;

        // this class should not "own" the resolver, but this does tightly couple it to the client initalisation so on a higher level we don't risk null exceptions.
        public IDeployUserResolver DeployUserResolver { get; }

        public DeployStatusInfoClient(DeployStatusConfiguration configuration)
        {
            octopusClient = new OctopusClient(configuration.Octopus);
            trelloClient = new TrelloClient(configuration.Trello);
            teamCityClient = new TeamCityClient(configuration.TeamCity);
            DeployUserResolver = configuration.DeployUserResolver;
        }

        public async Task<IEnumerable<DeployStatusInfo>> GetDeployStatus()
        {
            var environments = octopusClient.GetEnvironments().ToList();
            var tasks = new List<Task<DeployStatusInfo>>();
            foreach (var environment in environments)
            {
                tasks.Add(GetDeployStatusInfo(teamCityClient, environment, trelloClient));
            }

            return await Task.WhenAll(tasks);
        }

        private static async Task<DeployStatusInfo> GetDeployStatusInfo(TeamCityClient teamCityClient, OctopusEnvironmentInfo environment,
            TrelloClient trelloClient)
        {
            var buildInfo = (await teamCityClient.GetBuildsContaining(new Version(environment.ReleaseVersion))).ToList();

            var branchName = GetBranchNameFromReleaseNotes(environment.ReleaseNotes);
            if (string.IsNullOrWhiteSpace(branchName) && buildInfo.Any())
                branchName = buildInfo.First(x => !string.IsNullOrWhiteSpace(x.BranchName)).BranchName;

            var trelloCards = Enumerable.Empty<TrelloCardInfo>();
            if (!string.IsNullOrWhiteSpace(branchName))
                trelloCards = await trelloClient.GetCardsLinkedToBranch(branchName);

            return new DeployStatusInfo(environment, buildInfo, trelloCards, branchName);
        }

        private static string GetBranchNameFromReleaseNotes(string releaseNotes)
        {
            if (string.IsNullOrWhiteSpace(releaseNotes))
                return releaseNotes;

            var split = releaseNotes.Split('-');
            var allExceptReleaseVersionAndDeployVersionAndGitSha = split.Take(split.Count() - 3);
            return string.Join("-", allExceptReleaseVersionAndDeployVersionAndGitSha);
        }
    }
}