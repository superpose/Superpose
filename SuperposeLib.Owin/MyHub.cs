using System;
using Microsoft.AspNet.SignalR;
using Superpose.StorageInterface;
using SuperposeLib.Core;

namespace SuperposeLib.Owin
{
    public class MyHub : Hub
    {
        public void QueueSampleJob()
        {
            int total = 1000;
            for (int i = 0; i < total; i++)
            {
                JobHandler.EnqueueJob<TestJob>();
            }
            GetJobStatistics();
        }

        public void GetJobStatistics()
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage())
            {
                var jobStatistics = storage.JobLoader.GetJobStatistics();
                Clients.All.jobStatisticsCompleted(jobStatistics);
            }

        }
    }

    public class TestJob:AJob
    {
        protected override void Execute()
        {
           // Console.WriteLine("woooo!");
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