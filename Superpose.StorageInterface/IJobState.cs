using System;
using System.Collections.Generic;

namespace Superpose.StorageInterface
{
    public interface IJobState
    {
        string JobStateTypeName { set; get; }
        DateTime? Started { set; get; }
        DateTime? Ended { set; get; }

        /// <summary>
        ///     updated in process
        /// </summary>
        List<JobExecutionStatus> PreviousJobExecutionStatusList { set; get; }
    }
}