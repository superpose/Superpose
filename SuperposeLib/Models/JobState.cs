using System;
using System.Collections.Generic;
using Superpose.StorageInterface;

namespace SuperposeLib.Models
{
    public class JobState : IJobState
    {
        public string JobStateTypeName { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? LastUpdated { get; set; }
        public IList<JobExecutionStatus> PreviousJobExecutionStatusList { get; set; }
    }
}