using System;
using System.Collections.Generic;
using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Models
{
    public class JobLoad: JobPartOfJobLoad, IJobLoad
    {
        public DateTime? TimeToRun { get; set; }
        public Type JobType { get; set; }
        public string JobId { get; set; }
        public JobStateType JobStateType { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }
        public List<JobExecutionStatus> PreviousJobExecutionStatusList   { get; set; }
    }
}   