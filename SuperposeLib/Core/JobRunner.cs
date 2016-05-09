using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public class JobRunner : IJobRunner, IDisposable
    {
        private readonly IJobConverter _jobConverter;
        private readonly IJobStorage _jobStorage;
        private readonly ITime _time;

        public JobRunner(IJobStorage jobStorage, IJobConverter jobConverter, ITime time = null)
        {
            _jobStorage = jobStorage;
            _jobConverter = jobConverter;
            _time = time;
        }

        private Timer Timer { set; get; }

        public void Dispose()
        {
            Timer.Dispose();
        }



        public bool Run(Action<string> onRunning, Action<string> runningCompleted)
        {
            JobFactory = new JobFactory(_jobStorage, _jobConverter, _time);
            var queueName = SuperposeGlobalConfiguration.JobQueue.GetType().Name;
            var queue = SuperposeGlobalConfiguration.JobQueue;

            var hasNoWorkToDo = true;

          
            try
            {
                var jobsIds=new List<string>();
                if (!SuperposeGlobalConfiguration.StopProcessing)
                {
                
                      jobsIds = JobFactory
                    .JobStorage
                    .JobLoader
                    .LoadJobsByJobStateTypeAndTimeToRun(queueName,
                        JobStateType.Queued,
                        JobFactory.Time.MinValue,
                        JobFactory.Time.UtcNow.AddMinutes(1), queue.MaxNumberOfJobsPerLoad, 0);
                hasNoWorkToDo = jobsIds == null || jobsIds.Count == 0;
                }
               

                if (hasNoWorkToDo)
                {
                    Task.Delay(TimeSpan.FromSeconds(queue.StorgePollSecondsInterval))
                        .ContinueWith(c => Run(onRunning, runningCompleted));
                }
                else
                {
                    var cts = new CancellationTokenSource();
                    var po = new ParallelOptions
                    {
                        CancellationToken = cts.Token,
                        MaxDegreeOfParallelism = queue.WorkerPoolCount
                    };
                    ParallelDoSomeWork(onRunning, runningCompleted, jobsIds, po, cts);
                    Run(onRunning, runningCompleted);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public IJobFactory JobFactory { get; set; }

        private void ParallelDoSomeWork(Action<string> onRunning, Action<string> runningCompleted, List<string> jobsIds, ParallelOptions po, CancellationTokenSource cts)
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