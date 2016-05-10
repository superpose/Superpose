using System;
using System.Collections.Generic;

namespace Superpose.StorageInterface
{
    public interface IJobLoad : IJobState
    {
        string QueuedOnServer { get; set; }
        string LastUpdatedOnServer { get; set; }
        DateTime? QueuedAt { set; get; }
        DateTime? TimeToRun { set; get; }
        string JobTypeFullName { set; get; }
         string JobName { get; set; }
        string Id { get; set; }
        JobQueue JobQueue { set; get; }
        string JobQueueName { set; get; }
        string Command { get; set; }
        string JobCommandTypeFullName { set; get; }
        List<string> NextCommand { set; get; }
    }
}