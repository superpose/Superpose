using System;
using System.Collections.Generic;

namespace Superpose.StorageInterface
{
    public class SerializableJobLoad : IJobLoad
    {
        public string Queue { get; set; }
        public string JobStateTypeName { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }
        public List<JobExecutionStatus> PreviousJobExecutionStatusList { get; set; }
        public DateTime? TimeToRun { get; set; }
        public string JobTypeFullName { get; set; }
        public string Id { get; set; }
        public JobQueue JobQueue { get; set; }
        public string JobQueueName { get; set; }
        public string Command { get; set; }
        public string JobCommandTypeFullName { get; set; }
        public string NextCommand { get; set; }
    }
}