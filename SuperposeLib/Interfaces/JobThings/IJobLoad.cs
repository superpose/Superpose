using System;

namespace SuperposeLib.Interfaces.JobThings
{
    public interface IJobLoad: IJobState
    {
        DateTime? TimeToRun { set; get; }

        string JobTypeFullName { set; get; }

  

        string Id { get; set; }

    }
}