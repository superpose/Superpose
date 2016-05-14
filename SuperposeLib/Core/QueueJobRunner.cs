using System;
using System.Threading;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public class QueueJobRunner : IJobRunner, IDisposable
    {
        private readonly IJobConverter _jobConverter;
        private readonly IJobStorage _jobStorage;
        private readonly ITime _time;

        public QueueJobRunner(IJobStorage jobStorage, IJobConverter jobConverter, ITime time = null)
        {
            _jobStorage = jobStorage;
            _jobConverter = jobConverter;
            _time = time;
        }


        public Timer Timer { get; set; }
        public IJobFactory JobFactory { get; set; }
        public bool Run(Action<string> onRunning, Action<string> runningCompleted)
        {
            JobFactory = new JobFactory(_jobStorage, _jobConverter, _time);
            var queueName = SuperposeGlobalConfiguration.JobQueue.GetType().Name;
            var queue = SuperposeGlobalConfiguration.JobQueue;
            throw new NotImplementedException();

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}