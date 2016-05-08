using System;
using Superpose.StorageInterface;
using SuperposeLib.Core;
using SuperposeLib.Models;

namespace SuperposeLib.Interfaces.JobThings
{
    public interface IJobFactory
    {
        ITime Time { set; get; }

        IJobStorage JobStorage { set; get; }
        string QueueJob(Type jobType, AJobCommand command = null, JobQueue jobQueue = null, string nextJob = null);
        JobLoad GetJobLoad(string jobId);
        string QueueJob<T>(AJobCommand command = null, JobQueue jobQueue = null, string nextJob = null);
        IJobLoad ProcessJob(string jobId);

        string ScheduleJob<T>(AJobCommand command = null, DateTime? scheduleTime = null,
            JobQueue jobQueue = null, string nextJob = null);

        JobLoad InstantiateJobComponent(IJobLoad jobLoad);
        JobResult InstantiateAndRunJob(IJobLoad jobLoad);

        string ScheduleJob(Type type, AJobCommand command = null, DateTime? scheduleTime = null,
            JobQueue jobQueue = null, string nextJob = null);

        SerializedJobLoad PrepareScheduleJob(Type jobType, AJobCommand command = null, DateTime? scheduleTime = null,
            JobQueue jobQueue = null, string nextJob = null);
    }
}