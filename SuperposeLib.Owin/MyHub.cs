using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using Superpose.StorageInterface;
using SuperposeLib.Core;
//using MiniActor;

namespace SuperposeLib.Owin
{
    public class SuperposeLibHub : Hub
    {
        //private static MiniActor<object, bool> HubActor { set; get; }

        public async Task<bool> ClientsAll(object data, Action<object> opeartion)
        {
            //  opeartion(data);
            // HubActor = HubActor ?? new MiniActor<object, bool>();
            //return await HubActor.Ask(data, async (d,stateHandler) =>
            // {
            //     try
            //     {
            //         opeartion(d);
            //     }
            //     catch (Exception e)
            //     {
            //         throw;
            //     }
            //     return await Task.FromResult(true);
            // }, null);
            try
            {
                opeartion(data);
            }
            catch (Exception e)
            {
                throw;
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> GetCurrentQueue()
        {
            return await ClientsAll(SuperposeGlobalConfiguration.JobQueue, (o) =>
             {
                 Clients.All.currentQueue(o);
             });
            //  Clients.All.currentQueue(SuperposeGlobalConfiguration.JobQueue);
        }

        public async Task<bool> GetCurrentProcessingState()
        {
            return await ClientsAll(SuperposeGlobalConfiguration.StopProcessing, (o) =>
             {
                 Clients.All.currentProcessingState(o);
             });

            //  Clients.All.currentProcessingState(SuperposeGlobalConfiguration.StopProcessing);
        }

        public async Task<bool> StopProcessing(bool shouldStop)
        {
            SuperposeGlobalConfiguration.StopProcessing = shouldStop;
            return await GetCurrentProcessingState();
        }

        public async Task<bool> SetQueueMaxNumberOfJobsPerLoad(int maxNumberOfJobsPerLoad)
        {
            SuperposeGlobalConfiguration.JobQueue.MaxNumberOfJobsPerLoad = maxNumberOfJobsPerLoad;
            return await GetCurrentQueue();
        }

        public async Task<bool> SetQueueStorgePollSecondsInterval(int storgePollSecondsInterval)
        {
            SuperposeGlobalConfiguration.JobQueue.StorgePollSecondsInterval = storgePollSecondsInterval;
            return await GetCurrentQueue();
        }

        public async Task<bool> SetQueueWorkerPoolCount(int workerPoolCount)
        {
            SuperposeGlobalConfiguration.JobQueue.WorkerPoolCount = workerPoolCount;
            return await GetCurrentQueue();
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
                        }, EnqueueStrategy.Cpu);

            }

        }

        public async Task<bool> LoadJobsByJobStateTypeAndQueue(string stateType, int take = 20, int skip = 0, string queue = null)
        {
            using (
                var storage =
                    SuperposeGlobalConfiguration.StorageFactory.GetJobStorage(
                        SuperposeGlobalConfiguration.StorageFactory.GetCurrentExecutionInstance()))
            {
                var jobs = storage.JobLoader.LoadJobsByJobStateTypeAndQueue(queue ?? typeof(DefaultJobQueue).Name,
                    (JobStateType)Enum.Parse(typeof(JobStateType), stateType, true), take, skip);

                return await ClientsAll(jobs ?? new List<SerializableJobLoad>(), (o) =>
                 {
                     Clients.All.jobsList(o);
                 });
                // Clients.All.jobsList(jobs ?? new List<SerializableJobLoad>());
            }
        }

        public async Task<bool> LoadJobsByQueue(int take = 20, int skip = 0, string queue = null)
        {
            using (
                var storage =
                    SuperposeGlobalConfiguration.StorageFactory.GetJobStorage(
                        SuperposeGlobalConfiguration.StorageFactory.GetCurrentExecutionInstance()))
            {
                var jobs = storage.JobLoader.LoadJobsByQueue(queue ?? typeof(DefaultJobQueue).Name, take, skip);
                return await ClientsAll(jobs ?? new List<SerializableJobLoad>(), (o) =>
                {
                    Clients.All.jobsList(o);
                });
                // Clients.All.jobsList(jobs ?? new List<SerializableJobLoad>());
            }
        }

        public async Task<bool> GetJobStatistics()
        {
            using (
                var storage =
                    SuperposeGlobalConfiguration.StorageFactory.GetJobStorage(
                        SuperposeGlobalConfiguration.StorageFactory.GetCurrentExecutionInstance()))
            {
                var jobStatistics = storage.JobLoader.GetJobStatistics();
                return await ClientsAll(jobStatistics, (o) =>
                 {
                     Clients.All.jobStatisticsCompleted(o);
                 });
                //  Clients.All.jobStatisticsCompleted(jobStatistics);
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
            if (DateTime.Now.Second % 19 == 0)
                throw new Exception();

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