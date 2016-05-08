using System;
using Superpose.StorageInterface;
using SuperposeLib.Models;

namespace SuperposeLib.Interfaces.JobThings
{
    public interface IJobFactory
    {
        ITime Time { set; get; }

        IJobStorage JobStorage { set; get; }
        string QueueJob(Type jobType, object command=null, JobQueue jobQueue = null);
        JobLoad GetJobLoad(string jobId);
        string QueueJob<T>(object command = null, JobQueue jobQueue = null);
        IJobLoad ProcessJob(string jobId);

        string ScheduleJob<T>(object command = null, DateTime? scheduleTime = null,
            JobQueue jobQueue = null);
        JobLoad InstantiateJobComponent(IJobLoad jobLoad);
        string ScheduleJob(Type type, object command=null, DateTime? scheduleTime=null, JobQueue jobQueue = null);
    }
}