using System;
using System.Collections.Generic;
using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Models
{
    public class JobState: IJobState
    {
        public string JobStateTypeName { get; set; }
        public DateTime? Started { get; set; }  
        public DateTime? Ended { get; set; }
        public List<JobExecutionStatus> PreviousJobExecutionStatusList { get; set; }
    }
}