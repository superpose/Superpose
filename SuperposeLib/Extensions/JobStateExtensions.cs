using System.Linq;
using Superpose.StorageInterface;

namespace SuperposeLib.Extensions
{
    public static class JobStateExtensions
    {
        public static int HistoricFailureCount(this IJobState jobState)
        {
            var count= jobState.PreviousJobExecutionStatusList.Split(',').Count(x => x == JobExecutionStatus.Failed.ToStringName());
            return count;
        }
    }
}