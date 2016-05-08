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
            const int total = 1000000;
          
            for (var i = 0; i < total; i++)
            {
                JobHandler.EnqueueJob<TestJob>();
                // Parallel.Invoke(() => JobHandler.EnqueueJob<TestJob>());
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
        public override SuperVisionDecision Supervision(Exception reaon, int totalNumberOfHistoricFailures)
        {
            return SuperVisionDecision.Fail;
        }

        protected override void Execute()
        { 
            if(DateTime.Now.Second%19==0)
            throw  new Exception();

          //  Task.WaitAll(Task.Delay(TimeSpan.FromMilliseconds(10)));
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