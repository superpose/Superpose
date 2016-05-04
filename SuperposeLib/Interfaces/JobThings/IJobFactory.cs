using System;
using System.Collections.Generic;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;

namespace SuperposeLib.Interfaces.JobThings
{
    public interface IJobFactory
    {
        string QueueJob(Type jobType);

         ITime Time { set; get; }
        JobLoad GetJobLoad(string jobId);

        IJobStorage JobStorage { set; get; }

        IJobLoad ProcessJob(string jobId);
        JobLoad InstantiateJobComponent(IJobLoad jobLoad);
        string ScheduleJob(Type type, DateTime? scheduleTime);
    }
}