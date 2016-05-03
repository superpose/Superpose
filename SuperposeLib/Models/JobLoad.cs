using SuperposeLib.Core;
using System;

namespace SuperposeLib.Models
{
    public class JobLoad
    {
        public DateTime? TimeToRun { set; get; }

        public Type JobType { set; get; }

        public JobState State { set; get; }

        public AJob Job { set; get; }

        public string JobId { get; set; }

        public string GetJobName()
        {
            return JobType.Name;
        }
    }
}