using System;
using System.Collections.Generic;

namespace Superpose.StorageInterface
{
    public interface IJobState
    {
        string JobStateTypeName { set; get; }
        DateTime? Started { set; get; }
        DateTime? LastUpdated { set; get; }

        /// <summary>
        ///     updated in process
        /// </summary>
        IList<JobExecutionStatus> PreviousJobExecutionStatusList { set; get; }
    }
}