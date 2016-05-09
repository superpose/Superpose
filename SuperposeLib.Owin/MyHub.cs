using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Superpose.StorageInterface;
using SuperposeLib.Core;

namespace SuperposeLib.Owin
{
   
    public class MyHub : Hub
    {
        public void GetCurrentQueue()
        {
            Clients.All.currentQueue(SuperposeGlobalConfiguration.JobQueue);
        }

        public void GetCurrentProcessingState()
        {
            Clients.All.currentProcessingState(SuperposeGlobalConfiguration.StopProcessing?"Not Processing": "Processing");
        }

        public void StopProcessing(bool shouldStop)
        {
            SuperposeGlobalConfiguration.StopProcessing = shouldStop;
            GetCurrentQueue();
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

        public class MyQueue : JobQueue
        {

        }

        public void QueueSampleJob()
        {
            const int total = 1;

            for (var i = 0; i < total; i++)
            {
                JobHandler.EnqueueJob((c) =>new List<string>()
                {
                     c.EnqueueJob(new MyQueue(), ()=>Console.WriteLine("what up")),
                     c.EnqueueJob(()=>Console.WriteLine("what up")),
                     c.EnqueueJob(()=>Console.WriteLine("what up")),
                     c.EnqueueJob(()=>Console.WriteLine("what up")),
                     c.EnqueueJob(()=>Console.WriteLine("what up"))
                });
                //JobHandler.EnqueueJob<TestJob>();
                // Parallel.Invoke(() => JobHandler.EnqueueJob<TestJob>());
            }
            GetJobStatistics();
        }

         public void GetJobsByJobStateType(string stateType, int take=20,int skip=0, string queue = null)
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage())
            {
                var jobs = storage.JobLoader.LoadJobIdsByJobStateType(queue?? typeof(DefaultJobQueue).Name, (JobStateType)Enum.Parse(typeof(JobStateType), stateType,true), take,skip);
                Clients.All.jobsList(jobs);
            }
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

    public class TestJob : AJob
    {
        public override SuperVisionDecision Supervision(Exception reaon, int totalNumberOfHistoricFailures)
        {
            return SuperVisionDecision.Fail;
        }

        protected override void Execute()
        {
            if (DateTime.Now.Second%19 == 0)
                throw new Exception();

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