using System;

namespace SuperposeLib.Interfaces.JobThings
{
    public interface IJobLoad: IJobState
    {
        DateTime? TimeToRun { set; get; }

        Type JobType { set; get; }

       // IJobState State { set; get; }

        string JobId { get; set; }
    }
}