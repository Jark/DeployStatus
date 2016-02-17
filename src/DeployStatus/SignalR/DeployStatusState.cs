using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using DeployStatus.ApiClients;
using DeployStatus.Configuration;
using Microsoft.AspNet.SignalR;

namespace DeployStatus.SignalR
{
    public class DeployStatusState
    {
        public static readonly Lazy<DeployStatusState> Instance =
            new Lazy<DeployStatusState>( () => new DeployStatusState(GlobalHost.ConnectionManager.GetHubContext<DeployStatusHub, IDeployStatusClient>()));

        private readonly object locker = new object();

        private DeploySystemStatus deploySystemStatus;
        private readonly IHubContext<IDeployStatusClient> context;
        private readonly Timer timer;
        private DeployStatusInfoClient deployStatusInfoClient;
        private DeployStatusConfiguration configuration;

        private DeployStatusState(IHubContext<IDeployStatusClient> context)
        {
            this.context = context;
            timer = new Timer(UpdateDeploySystemStatus);
        }

        public void Start(DeployStatusConfiguration configuration)
        {
            deployStatusInfoClient = new DeployStatusInfoClient(configuration);
            this.configuration = configuration;
            timer.Change(TimeSpan.FromMinutes(0), TimeSpan.FromMilliseconds(-1));
        }

        private async void UpdateDeploySystemStatus(object state)
        {
            try
            {
                var status = await deployStatusInfoClient.GetDeployStatus();
                var newEnvironments = GetEnvironments(status, deployStatusInfoClient.DeployUserResolver);
                var newDeploySystemStatus = new DeploySystemStatus(configuration.Name, DateTime.UtcNow, newEnvironments);
                lock (locker)
                {
                    deploySystemStatus = newDeploySystemStatus;
                }
                context.Clients.All.DeploySystemStatusChanged(deploySystemStatus);
            }
            catch (Exception ex)
            {
                Debug.Assert(true, ex.ToString());
            }

            timer.Change(TimeSpan.FromSeconds(15), TimeSpan.FromMilliseconds(-1));
        }

        public DeploySystemStatus GetDeploySystemStatus()
        {
            lock (locker)
            {
                return deploySystemStatus ?? new DeploySystemStatus("Starting system...", DateTime.UtcNow, Enumerable.Empty<Environment>());
            }
        }

        private static IList<Environment> GetEnvironments(IEnumerable<DeployStatusInfo> status, IDeployUserResolver deployUserResolver)
        {            
            return
                status.Select(
                    x =>
                        new Environment(x.Environment.Id, x.Environment.Name, x.Environment.ReleaseVersion,
                            x.Environment.StartTime.GetValueOrDefault(), 
                            x.Environment.State, x.BranchName, x.Environment.AbsoluteDeployLink, 
                            GetNormalizedName(deployUserResolver.GetDeployer(x)), x.TrelloCards.Select(GetTrelloCard).ToList(), 
                            x.BuildInfo.Select(GetBuildInfo))).ToList();
        }

        private static string GetNormalizedName(string deployerName)
        {
            var undottedName = deployerName.Replace('.', ' ');

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(undottedName);
        }

        private static Trello GetTrelloCard(TrelloCardInfo trelloCardInfo)
        {
            return new Trello(trelloCardInfo.Id, trelloCardInfo.Name, trelloCardInfo.Url);
        }

        private static TeamCityBuild GetBuildInfo(TeamCityBuildInfo buildInfo)
        {
            return new TeamCityBuild(buildInfo.Id, buildInfo.BuildTypeId, buildInfo.WebUrl, buildInfo.Status, buildInfo.TriggeredAt);
        }

        public void Stop()
        {
            timer.Dispose();
        }
    }
}