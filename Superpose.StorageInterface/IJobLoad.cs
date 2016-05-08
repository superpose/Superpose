using System;
using System.Collections.Generic;

namespace Superpose.StorageInterface
{
    public interface IJobLoad : IJobState
    {
        DateTime? TimeToRun { set; get; }
        string JobTypeFullName { set; get; }
        string Id { get; set; }
        JobQueue JobQueue { set; get; }
        string JobQueueName { set; get; }
        string Command { get; set; }
        string JobCommandTypeFullName { set; get; }
        List<string> NextCommand { set; get; }
    }
}