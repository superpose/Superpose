using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Superpose.StorageInterface;
using SuperposeLib.Core;

namespace SuperposeLib.Owin
{
    // SuperposeGlobalConfiguration.JobQueue
    public class MyHub : Hub
    {

        public void GetCurrentQueue()
        {
            Clients.All.currentQueue(SuperposeGlobalConfiguration.JobQueue);
        }
        public void SetQueueMaxNumberOfJobsPerLoad(int maxNumberOfJobsPerLoad)
        {
            SuperposeGlobalConfiguration.JobQueue.MaxNumberOfJobsPerLoad = maxNumberOfJobsPerLoad;
            GetCurrentQueue();
        }
        public void SetQueueStorgePollSecondsInterval(int storgePollSecondsInterval)
        {
            SuperposeGlobalConfiguration.JobQueue.StorgePollSecondsInterval = storgePollSecondsInterval;
            GetCurrentQueue();
        }
        public void SetQueueWorkerPoolCount(int workerPoolCount)
        {
            SuperposeGlobalConfiguration.JobQueue.WorkerPoolCount = workerPoolCount;
            GetCurrentQueue();
        }

        public void QueueSampleJob()
        {
            const int total = 1000;
            for (var i = 0; i < total; i++)
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
           // throw  new Exception();
            Task.WaitAll(Task.Delay(TimeSpan.FromMilliseconds(100)));
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