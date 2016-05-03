using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperposeLib.Models
{
    public class JobState
    {
        public JobState()
        {
        }

        public JobStateType JobStateType { set; get; }
        public DateTime? Started { set; get; }
        public DateTime? Ended { set; get; }

        /// <summary>
        /// updated in process
        /// </summary>
        public List<JobStatus> PreviousJobStatus { set; get; }
            
        public int HistoricFailureCount()
        {
            return PreviousJobStatus.Count(x => x == JobStatus.Failed);
        }
    }
}