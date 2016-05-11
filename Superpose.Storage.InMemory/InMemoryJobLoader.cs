using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Superpose.StorageInterface;
using SuperposeLib.Services.InMemoryStorage;

namespace Superpose.Storage.InMemory
{
    public class InMemoryJobLoader : IJobLoader
    {
        public string LoadJobById(string jobId)
        {
            SerializableJobLoad data = null;
            if (InMemoryJobStorageMemoryStore.MemoryStore.ContainsKey(jobId))
            {
                data = InMemoryJobStorageMemoryStore.MemoryStore[jobId];
            }
            return data == null ? null : JsonConvert.SerializeObject(data);
        }

        public JobStatistics GetJobStatistics()
        {
            return new JobStatistics
            {
                TotalNumberOfJobs = InMemoryJobStorageMemoryStore.MemoryStore.Count,
                TotalQueuedJobs =
                    InMemoryJobStorageMemoryStore.MemoryStore.Count(
                        x => x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Queued)),
                TotalProcessingJobs =
                    InMemoryJobStorageMemoryStore.MemoryStore.Count(
                        x => x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Processing)),
                TotalDeletedJobs =
                    InMemoryJobStorageMemoryStore.MemoryStore.Count(
                        x => x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Deleted)),
                TotalSuccessfullJobs =
                    InMemoryJobStorageMemoryStore.MemoryStore.Count(
                        x => x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Successfull)),
                TotalFailedJobs =
                    InMemoryJobStorageMemoryStore.MemoryStore.Count(
                        x => x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Failed)),
                TotalUnknownJobs =
                    InMemoryJobStorageMemoryStore.MemoryStore.Count(
                        x => x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Unknown))
            };
        }

        public List<string> LoadJobIdsByJobType(string queueName, Type jobType, int take, int skip)
        {
            return
                InMemoryJobStorageMemoryStore.MemoryStore.Where(
                    x => x.Value.JobTypeFullName == jobType.AssemblyQualifiedName && x.Value.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Key)
                    .ToList();
        }

        public List<string> LoadJobIdsByJobStateType(string queueName, JobStateType stateType, int take, int skip)
        {
            return
                InMemoryJobStorageMemoryStore.MemoryStore.Where(
                    x =>
                        x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), stateType) &&
                        x.Value.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Key)
                    .ToList();
        }

        public List<SerializableJobLoad> LoadJobsByJobStateTypeAndQueue(string queueName, JobStateType stateType,
            int take, int skip)
        {
            return
                InMemoryJobStorageMemoryStore.MemoryStore.Where(
                    x =>
                        x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), stateType) &&
                        x.Value.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Value)
                    .ToList();
        }

        public List<SerializableJobLoad> LoadJobsByQueue(string queueName, int take, int skip)
        {
            return
                InMemoryJobStorageMemoryStore.MemoryStore.Where(
                    x =>
                        x.Value.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Value)
                    .ToList();
        }

        public List<SerializableJobLoad> LoadJobs(int take, int skip)
        {
            return
                InMemoryJobStorageMemoryStore.MemoryStore
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Value)
                    .ToList();
        }


        public List<string> LoadJobIdsByTimeToRun(string queueName, DateTime @from, DateTime to, int take, int skip)
        {
            return InMemoryJobStorageMemoryStore.MemoryStore.Where
                (x => x.Value.TimeToRun >= @from && x.Value.TimeToRun <= to && x.Value.JobQueueName == queueName)
                .Take(take)
                .Skip(skip)
                .Select(x => x.Key)
                .ToList();
        }

        public List<string> LoadJobIdsByJobStateTypeAndTimeToRun(string queueName, JobStateType stateType,
            DateTime @from,
            DateTime to,
            int take, int skip)
        {
            return InMemoryJobStorageMemoryStore.MemoryStore
                .Where(x =>
                    x.Value.TimeToRun >= @from
                    && x.Value.TimeToRun <= to &&
                    x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), stateType) &&
                    x.Value.JobQueueName == queueName)
                .Skip(skip)
                .Take(take)
                .Select(x => x.Key)
                .ToList();
        }

        public List<string> LoadJobsByIds(List<string> ids)
        {
            return ids.Select(LoadJobById).ToList();
        }

        public void Dispose()
        {
            //  InMemoryJobStorageMemoryStore.MemoryStore = new ConcurrentDictionary<string, SerializableJobLoad>();
        }
    }
}