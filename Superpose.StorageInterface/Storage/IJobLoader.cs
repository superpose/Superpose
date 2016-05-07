using System;
using System.Collections.Generic;

namespace Superpose.StorageInterface.Storage
{
    public interface IJobLoader : IDisposable
    {
        string LoadJobById(string jobId);
        JobStatistics GetJobStatistics();
        List<string> LoadJobsByJobType(Type jobType, int take, int skip);
        List<string> LoadJobsByJobStateType(JobStateType stateType, int take, int skip);
        List<string> LoadJobsByTimeToRun(DateTime from, DateTime to, int take, int skip);

        List<string> LoadJobsByJobStateTypeAndTimeToRun(JobStateType stateType, DateTime from, DateTime to, int take,
            int skip);

        List<string> LoadJobsByIds(List<string> ids);
    }
}