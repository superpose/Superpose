using System;
using Superpose.StorageInterface;
using SuperposeLib.Models;

namespace SuperposeLib.Interfaces.JobThings
{
    public interface IJobFactory
    {
        ITime Time { set; get; }

        IJobStorage JobStorage { set; get; }
        string QueueJob(Type jobType, IJobCommand command =null, JobQueue jobQueue = null);
        JobLoad GetJobLoad(string jobId);
        string QueueJob<T>(IJobCommand command = null, JobQueue jobQueue = null);
        IJobLoad ProcessJob(string jobId);

        string ScheduleJob<T>(IJobCommand command = null, DateTime? scheduleTime = null,
            JobQueue jobQueue = null);

        JobLoad InstantiateJobComponent(IJobLoad jobLoad);
        JobResult InstantiateAndRunJob(IJobLoad jobLoad);
        string ScheduleJob(Type type, IJobCommand command =null, DateTime? scheduleTime=null, JobQueue jobQueue = null);
    }
}