using System;
using System.Collections.Generic;
using Superpose.StorageInterface;

namespace SuperposeLib.Models
{
    public class JobLoad : JobPartOfJobLoad, IJobLoad
    {
        public JobLoad()
        {
            NextCommand = new List<string>();
        }

        public string Queue { get; set; }
        public string QueuedOnServer { get; set; }
        public string LastUpdatedOnServer { get; set; }
        public DateTime? QueuedAt { get; set; }
        public DateTime? TimeToRun { get; set; }
        public string JobTypeFullName { get; set; }
        public string JobName { get; set; }
        public string Id { get; set; }
        public JobQueue JobQueue { get; set; }
        public string JobQueueName { get; set; }
        public string Command { get; set; }
        public string JobCommandTypeFullName { get; set; }
        public List<string> NextCommand { get; set; }
        public string JobStateTypeName { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? LastUpdated { get; set; }
        public IList<JobExecutionStatus> PreviousJobExecutionStatusList { get; set; }
    }
}