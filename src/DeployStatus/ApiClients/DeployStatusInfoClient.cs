using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeployStatus.Configuration;
using log4net;

namespace DeployStatus.ApiClients
{
    public class DeployStatusInfoClient
    {
        private readonly OctopusClient octopusClient;
        private readonly TrelloClient trelloClient;
        private readonly TeamCityClient teamCityClient;
        private readonly ILog log;

        // this class should not "own" the resolver, but this does tightly couple it to the client initalisation so on a higher level we don't risk null exceptions.
        public IDeployUserResolver DeployUserResolver { get; }

        public DeployStatusInfoClient(DeployStatusConfiguration configuration)
        {
            octopusClient = new OctopusClient(configuration.Octopus);
            trelloClient = new TrelloClient(configuration.Trello);
            teamCityClient = new TeamCityClient(configuration.TeamCity);
            DeployUserResolver = configuration.DeployUserResolver;

            log = LogManager.GetLogger(typeof (DeployStatusInfoClient));
        }

        public async Task<IEnumerable<DeployStatusInfo>> GetDeployStatus()
        {
            log.Info("Retrieving environments...");
            var environments = octopusClient.GetEnvironments().ToList();

            log.Info("Queueing and starting getting details tasks...");
            var tasks = new List<Task<DeployStatusInfo>>();
            foreach (var environment in environments)
            {
                tasks.Add(GetDeployStatusInfo(teamCityClient, environment, trelloClient));
            }

            log.Info("Waiting for tasks to complete.");

            await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromMinutes(15)));

            if (tasks.All(x => x.IsCompleted))
                return await Task.WhenAll(tasks);

            throw new Exception("Could not get the deploy statuses in time.");            
        }

        private async Task<DeployStatusInfo> GetDeployStatusInfo(TeamCityClient teamCityClient, OctopusEnvironmentInfo environment,
            TrelloClient trelloClient)
        {
            var buildInfo = (await teamCityClient.GetBuildsContaining(new Version(environment.ReleaseVersion))).ToList();

            var branchName = GetBranchNameFromReleaseNotes(environment.ReleaseNotes);
            if (string.IsNullOrWhiteSpace(branchName) && buildInfo.Any())
                branchName = buildInfo.First(x => !string.IsNullOrWhiteSpace(x.BranchName)).BranchName;

            var branchRelatedTrelloCards = Enumerable.Empty<TrelloCardInfo>();
            if (!string.IsNullOrWhiteSpace(branchName))
                branchRelatedTrelloCards = await trelloClient.GetCardsLinkedToBranch(branchName);

            var labelRelatedCards = await trelloClient.GetCardsLinkedToLabel(environment.Name);

            return new DeployStatusInfo(environment, buildInfo, branchRelatedTrelloCards, labelRelatedCards, branchName);
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