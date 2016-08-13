using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Core;
using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.JobThings;
using MiniActor;

namespace Superpose.JobRunnerInterface
{
    public class DefaultJobRunner : IJobRunner, IDisposable
    {
        private readonly IJobConverter _jobConverter;
        private readonly IJobStorage _jobStorage;
        private readonly ITime _time;
        private MiniActor<string, bool> RunActor = new MiniActor<string,bool>();
        private MiniActor<string, List<string>> QueryActor = new MiniActor<string, List<string>>();
        public DefaultJobRunner(IJobStorage jobStorage, IJobConverter jobConverter, ITime time = null)
        {
            _jobStorage = jobStorage;
            _jobConverter = jobConverter;
            _time = time;
            JobFactory = new JobFactory(_jobStorage, _jobConverter, _time);
        }

        public Timer Timer { set; get; }

        public void Dispose()
        {
            Timer.Dispose();
        }


        public async Task<bool> RunAsync(Action<string> onRunning, Action<string> runningCompleted)
        {
            var queueName = SuperposeGlobalConfiguration.JobQueue.GetType().Name;
            var queue = SuperposeGlobalConfiguration.JobQueue;

            var hasNoWorkToDo = true;


            try
            {
                var jobsIds = new List<string>();
                if (!SuperposeGlobalConfiguration.StopProcessing)
                {

                jobsIds =  await QueryActor.Ask(queueName,async (m) => await  Task.FromResult(JobFactory
                    .JobStorage
                    .JobLoader
                    .LoadJobIdsByJobStateTypeAndTimeToRun(m,
                        JobStateType.Queued,
                        JobFactory.Time.MinValue,
                        JobFactory.Time.UtcNow.AddMinutes(1), queue.MaxNumberOfJobsPerLoad, 0)));
                    hasNoWorkToDo = jobsIds == null || jobsIds.Count == 0;
                    //jobsIds = JobFactory
                    //    .JobStorage
                    //    .JobLoader
                    //    .LoadJobIdsByJobStateTypeAndTimeToRun(queueName,
                    //        JobStateType.Queued,
                    //        JobFactory.Time.MinValue,
                    //        JobFactory.Time.UtcNow.AddMinutes(1), queue.MaxNumberOfJobsPerLoad, 0);
                    //hasNoWorkToDo = jobsIds == null || jobsIds.Count == 0;
                }


                if (hasNoWorkToDo)
                {
                  await Task.Delay(TimeSpan.FromSeconds(queue.StorgePollSecondsInterval))
                        .ContinueWith(c => RunAsync(onRunning, runningCompleted));
                }
                else
                {
                    var cts = new CancellationTokenSource();
                    var po = new ParallelOptions
                    {
                        CancellationToken = cts.Token,
                        MaxDegreeOfParallelism = queue.WorkerPoolCount
                    };

                    await Task.WhenAll(jobsIds.Select( x => RunActor.Ask(x, async (m) =>
                    {
                        if (!SuperposeGlobalConfiguration.StopProcessing)
                        {
                            DoSomeWork(onRunning, runningCompleted, m);
                        }
                        return await Task.FromResult(true);
                    })));
                   // ParallelDoSomeWork(onRunning, runningCompleted, jobsIds, po, cts);
                 await   RunAsync(onRunning, runningCompleted);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public IJobFactory JobFactory { get; set; }

        private void ParallelDoSomeWork(Action<string> onRunning, Action<string> runningCompleted, List<string> jobsIds,
            ParallelOptions po, CancellationTokenSource cts)
        {
            try
            {
                Parallel.ForEach(jobsIds, po, jobsId =>
                {
                    if (!SuperposeGlobalConfiguration.StopProcessing)
                    {
                        DoSomeWork(onRunning, runningCompleted, jobsId);
                    }
                    po.CancellationToken.ThrowIfCancellationRequested();
                });
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                cts.Dispose();
            }
        }

        private void DoSomeWork(Action<string> onRunning, Action<string> runningCompleted, string jobsId)
        {
            try
            {
                onRunning?.Invoke(jobsId);
            }
            catch (Exception)
            {
                //
            }

            JobFactory.ProcessJob(jobsId);
            try
            {
                runningCompleted?.Invoke(jobsId);
            }
            catch (Exception)
            {
                //
            }
        }
    }
}