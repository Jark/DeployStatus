using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;

namespace DeployStatus.ApiClients
{
    public class OctopusClient
    {
        private readonly OctopusRepository repository;
        private readonly Uri serverUri;
        
        public OctopusClient(OctopusApiConfiguration octopusApiConfiguration)
        {
            repository = new OctopusRepository(new OctopusServerEndpoint(octopusApiConfiguration.ServerUri, octopusApiConfiguration.ApiKey));
            serverUri = new Uri(octopusApiConfiguration.ServerUri);
        }

        public IEnumerable<OctopusEnvironmentInfo> GetEnvironments()
        {
            var swanSystem = repository.Projects.FindByName("SwanSystem");
            var dashBoardResourceResult = repository.Dashboards.GetDynamicDashboard(new[] { swanSystem.Id }, new string[] { });

            foreach (var dashboardItemResource in dashBoardResourceResult.Items)
            {
                var environment = dashBoardResourceResult.Environments.First(x => x.Id == dashboardItemResource.EnvironmentId);

                var machines = repository.Machines.FindMany(x => x.EnvironmentIds.Contains(environment.Id));
                var release = repository.Releases.Get(dashboardItemResource.ReleaseId);
                var task = repository.Tasks.Get(dashboardItemResource.TaskId);
                var events = repository.Events.List(regardingDocumentId: dashboardItemResource.DeploymentId);
                var deploymentQueuedByUser = events.Items.Where(x => x.Category == "DeploymentQueued").Select(x => x.UserId).FirstOrDefault();
                var user = repository.Users.Get(deploymentQueuedByUser);

                yield return
                    new OctopusEnvironmentInfo(
                        environment.Id, environment.Name,
                        machines,
                        task.StartTime, task.Duration, task.ErrorMessage,
                        task.State, release.Version, release.ReleaseNotes, user?.DisplayName, user?.Username,
                        new Uri(serverUri, task.Link("Web")).ToString());
            }
        }
    }
}