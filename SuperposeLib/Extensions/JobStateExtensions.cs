using System.Linq;
using Superpose.StorageInterface;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Models;

namespace SuperposeLib.Extensions
{
    public static class JobStateExtensions
    {
        public static int HistoricFailureCount(this IJobState jobState)
        {
            return jobState.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Failed);
        }
    }
}