using Microsoft.AspNet.SignalR;
using Superpose.StorageInterface;

namespace SuperposeLib.Owin
{
    public class MyHub : Hub
    {

        public void GetJobStatistics()
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage())
            {
                var jobStatistics = storage.JobLoader.GetJobStatistics();
                Clients.All.jobStatisticsCompleted(jobStatistics);
            }

        }
    }

    public class SuperposeSignalRContext
    {
        public static IHubContext GetHubContext()
        {
            return GlobalHost.ConnectionManager.GetHubContext<MyHub>();
        }
    }
}