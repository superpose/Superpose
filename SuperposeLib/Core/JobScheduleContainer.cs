using System;
using System.Collections.Generic;
using Superpose.StorageInterface;

namespace SuperposeLib.Core
{

    public class JobScheduleContainer
    {
        public Type JobType { set; get; }
        public AJobCommand Command { set; get; }
        public DateTime? ScheduleTime { set; get; }
        public JobQueue JobQueue { set; get; }
        public List<string> NextJob { set; get; }
    }
}