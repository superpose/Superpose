namespace Superpose.StorageInterface
{
    public class JobStatistics
    {
        public int TotalNumberOfJobs { set; get; }
        public int TotalQueuedJobs { set; get; }
        public int TotalProcessingJobs { set; get; }
        public int TotalDeletedJobs { set; get; }
        public int TotalSuccessfullJobs { set; get; }
        public int TotalFailedJobs { set; get; }
        public int TotalUnknownJobs { set; get; }
    }
    public interface IJobCommand
    {

    }

    public class DefaultJobCommand : IJobCommand
    {

    }
}