using System;
using System.Collections.Generic;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Models
{
    public class SerializableJobLoad : IJobLoad
    {
        public string JobStateTypeName { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }
        public List<JobExecutionStatus> PreviousJobExecutionStatusList { get; set; }
        public DateTime? TimeToRun { get; set; }
        public string JobTypeFullName { get; set; }
        public string Id { get; set; }
    }
}