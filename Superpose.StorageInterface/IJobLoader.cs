using System;
using System.Collections.Generic;

namespace Superpose.StorageInterface
{
    public interface IJobLoader : IDisposable
    {
        string LoadJobById(string jobId);
        JobStatistics GetJobStatistics();
        List<string> LoadJobsByJobType(string queueName, Type jobType, int take, int skip);
        List<string> LoadJobsByJobStateType(string queueName, JobStateType stateType, int take, int skip);
        List<string> LoadJobsByTimeToRun(string queueName, DateTime from, DateTime to, int take, int skip);
        List<string> LoadJobsByJobStateTypeAndTimeToRun(string queueName, JobStateType stateType, DateTime from, DateTime to, int take,int skip);
        List<string> LoadJobsByIds( List<string> ids);
    }
}