using System;

namespace Superpose.StorageInterface
{
    public interface IJobLoad : IJobState
    {
        DateTime? TimeToRun { set; get; }
        string JobTypeFullName { set; get; }
        string Id { get; set; } 
        JobQueue JobQueue { set; get; }
        string JobQueueName { set; get; }
    }
}