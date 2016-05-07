using System;
using System.Linq;
using System.Threading;
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

        public bool Run(Action<string> onRunning)
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

                if (jobsId == null || jobsId.Count != 1)
                {
                    Timer?.Dispose();
                    Timer = new Timer(state =>
                    {
                        Run(onRunning);
                    }, null, TimeSpan.FromSeconds(3), TimeSpan.Zero);
                    return true;
                }
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
                return Run(onRunning);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IJobFactory JobFactory { get; }
    }
}