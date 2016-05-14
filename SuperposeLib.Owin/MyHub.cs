using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Superpose.StorageInterface;
using SuperposeLib.Core;

namespace SuperposeLib.Owin
{
    public class SuperposeLibHub : Hub
    {
        public void GetCurrentQueue()
        {
            Clients.All.currentQueue(SuperposeGlobalConfiguration.JobQueue);
        }

        public void GetCurrentProcessingState()
        {
            Clients.All.currentProcessingState(SuperposeGlobalConfiguration.StopProcessing);
        }

        public void StopProcessing(bool shouldStop)
        {
            SuperposeGlobalConfiguration.StopProcessing = shouldStop;
            GetCurrentProcessingState();
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
                 const int total = 100000;
                    for (var i = 0; i < total; i++)
                    {
                        JobHandler.EnqueueJob(c => new List<string>
                        {
                            c.EnqueueJob(new MyQueue(), () => Console.WriteLine("what up")),
                            c.EnqueueJob(() => Console.WriteLine("what up")),
                            c.EnqueueJob(() => Console.WriteLine("what up")),
                            c.EnqueueJob(() => Console.WriteLine("what up")),
                            c.EnqueueJob(() => Console.WriteLine("what up")),
                            c.EnqueueJob(() => Console.WriteLine("what up")),
                            c.EnqueueJob(() => Console.WriteLine("what up")),
                            c.EnqueueJob(() => Console.WriteLine("what up")),
                            c.EnqueueJob(() => Console.WriteLine("what up")),
                            c.EnqueueJob(() => Console.WriteLine("what up")),
                            c.EnqueueJob(() => Console.WriteLine("what up"))
                        },EnqueueStrategy.Cpu);
                    
                    }
            
        }

        public void LoadJobsByJobStateTypeAndQueue(string stateType, int take = 20, int skip = 0, string queue = null)
        {
            using (
                var storage =
                    SuperposeGlobalConfiguration.StorageFactory.GetJobStorage(
                        SuperposeGlobalConfiguration.StorageFactory.GetCurrentExecutionInstance()))
            {
                var jobs = storage.JobLoader.LoadJobsByJobStateTypeAndQueue(queue ?? typeof (DefaultJobQueue).Name,
                    (JobStateType) Enum.Parse(typeof (JobStateType), stateType, true), take, skip);
                Clients.All.jobsList(jobs ?? new List<SerializableJobLoad>());
            }
        }

        public void LoadJobsByQueue(int take = 20, int skip = 0, string queue = null)
        {
            using (
                var storage =
                    SuperposeGlobalConfiguration.StorageFactory.GetJobStorage(
                        SuperposeGlobalConfiguration.StorageFactory.GetCurrentExecutionInstance()))
            {
                var jobs = storage.JobLoader.LoadJobsByQueue(queue ?? typeof (DefaultJobQueue).Name, take, skip);
                Clients.All.jobsList(jobs ?? new List<SerializableJobLoad>());
            }
        }

        public void GetJobStatistics()
        {
            using (
                var storage =
                    SuperposeGlobalConfiguration.StorageFactory.GetJobStorage(
                        SuperposeGlobalConfiguration.StorageFactory.GetCurrentExecutionInstance()))
            {
                var jobStatistics = storage.JobLoader.GetJobStatistics();
                Clients.All.jobStatisticsCompleted(jobStatistics);
            }
        }

        public class MyQueue : JobQueue
        {
        }
    }


    public class TestJob2 : AJob
    {
        protected override void Execute()
        {
            Task.WaitAll(Task.Delay(TimeSpan.FromMilliseconds(10)));
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
            return GlobalHost.ConnectionManager.GetHubContext<SuperposeLibHub>();
        }
    }
}