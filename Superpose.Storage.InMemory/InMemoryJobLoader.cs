using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;

namespace SuperposeLib.Services.InMemoryStorage
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

        public List<string> LoadJobsByJobType(Type jobType, int take, int skip)
        {
            return
                InMemoryJobStorageMemoryStore.MemoryStore.Where(
                    x => x.Value.JobTypeFullName == jobType.AssemblyQualifiedName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Key)
                    .ToList();
        }

        public List<string> LoadJobsByJobStateType(JobStateType stateType, int take, int skip)
        {
            return
                InMemoryJobStorageMemoryStore.MemoryStore.Where(
                    x => x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), stateType))
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Key)
                    .ToList();
        }

        public List<string> LoadJobsByTimeToRun(DateTime @from, DateTime to, int take, int skip)
        {
            return InMemoryJobStorageMemoryStore.MemoryStore.Where
                (x => x.Value.TimeToRun >= @from && x.Value.TimeToRun <= to)
                .Take(take)
                .Skip(skip)
                .Select(x => x.Key)
                .ToList();
        }

        public List<string> LoadJobsByJobStateTypeAndTimeToRun(JobStateType stateType, DateTime @from, DateTime to,
            int take, int skip)
        {
            return InMemoryJobStorageMemoryStore.MemoryStore
                .Where(x =>
                    x.Value.TimeToRun >= @from
                    && x.Value.TimeToRun <= to &&
                    x.Value.JobStateTypeName == Enum.GetName(typeof (JobStateType), stateType))
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