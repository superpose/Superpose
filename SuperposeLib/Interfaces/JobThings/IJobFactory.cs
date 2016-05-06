using System;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;

namespace SuperposeLib.Interfaces.JobThings
{
    public interface IJobFactory
    {
        ITime Time { set; get; }

        IJobStorage JobStorage { set; get; }
        string QueueJob(Type jobType);
        JobLoad GetJobLoad(string jobId);

        IJobLoad ProcessJob(string jobId);
        JobLoad InstantiateJobComponent(IJobLoad jobLoad);
        string ScheduleJob(Type type, DateTime? scheduleTime);
    }
}