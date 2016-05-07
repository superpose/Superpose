using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Models;

namespace SuperposeLib.Core
{
    public class JobRunner : IJobRunner, IDisposable
    {
        private readonly IJobStorage _jobStorage;
        private readonly IJobConverter _jobConverter;
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
            try
            {

                var jobsIds = JobFactory
                    .JobStorage
                    .JobLoader
                    .LoadJobsByJobStateTypeAndTimeToRun(
                        JobStateType.Queued,
                        JobFactory.Time.MinValue,
                        JobFactory.Time.UtcNow.AddMinutes(1), 100, 0);
                var hasNoWorkToDo = jobsIds == null || jobsIds.Count == 0;

                if (hasNoWorkToDo)
                {
                    Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(c => Run(onRunning, runningCompleted));
                }
                else
                {
                    //Environment.ProcessorCount
                    ParallelDoSomeWork(100, onRunning, runningCompleted, jobsIds);
                    Run(onRunning, runningCompleted);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        private void ParallelDoSomeWork(int maxDegreeOfParallelism,Action<string> onRunning, Action<string> runningCompleted, List<string> jobsIds)
        {
           
            Parallel.ForEach( jobsIds, new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            }, (jobsId) =>
            {
               // Task.WaitAll(Task.Delay(TimeSpan.FromSeconds(1)));
                DoSomeWork(onRunning, runningCompleted, jobsId);

            });
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

        public IJobFactory JobFactory { get; set; }
    }
}