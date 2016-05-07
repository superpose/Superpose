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
        public JobRunner(IJobStorage jobStorage, IJobConverter jobConverter, ITime time = null)
        {
            JobFactory = new JobFactory(jobStorage, jobConverter, time);
        }

        private Timer Timer { set; get; }

        public void Dispose()
        {
            Timer.Dispose();
        }

       
        public bool Run(Action<string> onRunning, Action<string> runningCompleted)
        {
            
            try
            {
                var jobsId = JobFactory
                    .JobStorage
                    .JobLoader
                    .LoadJobsByJobStateTypeAndTimeToRun(
                        JobStateType.Queued,
                        JobFactory.Time.MinValue,
                        JobFactory.Time.UtcNow.AddMinutes(1), 1, 0);
                var hasNoWorkToDo = jobsId == null || jobsId.Count != 1;

                if (hasNoWorkToDo)
                {
                    Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(c => Run(onRunning, runningCompleted));
                }
                else
                {
                    DoSomeWork(onRunning, runningCompleted, jobsId);
                    Task.Delay(TimeSpan.FromMilliseconds(10)).ContinueWith(c => Run(onRunning, runningCompleted));
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void DoSomeWork(Action<string> onRunning, Action<string> runningCompleted, List<string> jobsId)
        {
            var selectJob = jobsId.First();

            try
            {
                onRunning?.Invoke(selectJob);
            }
            catch (Exception)
            {
                //
            }

            JobFactory.ProcessJob(selectJob);
            try
            {
                runningCompleted?.Invoke(selectJob);
            }
            catch (Exception)
            {
                //
            }
        }

        public IJobFactory JobFactory { get; }
    }
}