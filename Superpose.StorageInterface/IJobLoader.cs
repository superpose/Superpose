using System;
using System.Collections.Generic;

namespace Superpose.StorageInterface
{
    public interface IJobLoader : IDisposable
    {
        string LoadJobById(string jobId);
        JobStatistics GetJobStatistics();
        List<string> LoadJobIdsByJobType(string queueName, Type jobType, int take, int skip);
        List<string> LoadJobIdsByJobStateType(string queueName, JobStateType stateType, int take, int skip);
        List<string> LoadJobIdsByTimeToRun(string queueName, DateTime from, DateTime to, int take, int skip);

        List<string> LoadJobIdsByJobStateTypeAndTimeToRun(string queueName, JobStateType stateType, DateTime from,
            DateTime to, int take, int skip);

        List<string> LoadJobsByIds(List<string> ids);

        List<SerializableJobLoad> LoadJobsByJobStateTypeAndQueue(string queueName, JobStateType stateType, int take,
            int skip);

        List<SerializableJobLoad> LoadJobsByQueue(string queueName, int take, int skip);
        List<SerializableJobLoad> LoadJobs(int take, int skip);
    }
}