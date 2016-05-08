using System;

namespace Superpose.StorageInterface
{
    public class JobQueue
    {
        public JobQueue()
        {
            WorkerPoolCount = Environment.ProcessorCount*10;
            MaxNumberOfJobsPerLoad = 100;
            StorgePollSecondsInterval = 1;
        }

        public int WorkerPoolCount { get; set; }
        public int MaxNumberOfJobsPerLoad { get; set; }
        public double StorgePollSecondsInterval { get; set; }
    }
}