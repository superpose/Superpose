using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;
using System;

namespace SuperposeLib.Interfaces
{
    public interface IJobFactory
    {
        string QueueJob(Type jobType);

        JobLoad GetJobLoad(string jobId);

        IJobStorage JobStorage { set; get; }

        JobLoad ProcessJob(string jobId);
        JobLoad InstantiateJobComponent(JobLoad jobLoad);
        string ScheduleJob(Type type, DateTime? scheduleTime);
    }
}