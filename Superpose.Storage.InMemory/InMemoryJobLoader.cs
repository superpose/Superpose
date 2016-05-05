using System;
using SuperposeLib.Interfaces.Storage;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SuperposeLib.Models;

namespace SuperposeLib.Services.InMemoryStorage
{
    public class InMemoryJobLoader : IJobLoader
    {
        public string LoadJobById(string jobId)
        {
            JobLoad data = null;
            if (InMemoryJobStorageMemoryStore.MemoryStore.ContainsKey(jobId))
            {
                data = InMemoryJobStorageMemoryStore.MemoryStore[jobId];
            }
            return data==null?null: JsonConvert.SerializeObject(data);
        }

        public JobStatistics GetJobStatistics()
        {
            return new JobStatistics()
            {
                TotalNumberOfJobs= InMemoryJobStorageMemoryStore.MemoryStore.Count,
                TotalQueuedJobs = InMemoryJobStorageMemoryStore.MemoryStore.Count(x => x.Value.JobStateType== JobStateType.Queued),
                TotalProcessingJobs = InMemoryJobStorageMemoryStore.MemoryStore.Count(x => x.Value.JobStateType == JobStateType.Processing),
                TotalDeletedJobs = InMemoryJobStorageMemoryStore.MemoryStore.Count(x => x.Value.JobStateType == JobStateType.Deleted),
                TotalSuccessfullJobs = InMemoryJobStorageMemoryStore.MemoryStore.Count(x => x.Value.JobStateType == JobStateType.Successfull),
                TotalFailedJobs = InMemoryJobStorageMemoryStore.MemoryStore.Count(x => x.Value.JobStateType == JobStateType.Failed),
                TotalUnknownJobs = InMemoryJobStorageMemoryStore.MemoryStore.Count(x => x.Value.JobStateType == JobStateType.Unknown)
            };
        }

        public List<string> LoadJobsByJobType(Type jobType, int take, int skip)
        {
           return InMemoryJobStorageMemoryStore.MemoryStore.Where(x => x.Value.JobType == jobType).Take(take).Skip(skip).Select(x=>x.Key).ToList();
        }

        public List<string> LoadJobsByJobStateType(JobStateType stateType, int take, int skip)
        {
            return InMemoryJobStorageMemoryStore.MemoryStore.Where(x => x.Value.JobStateType == stateType).Take(take).Skip(skip).Select(x => x.Key).ToList();
        }

        public List<string> LoadJobsByTimeToRun(DateTime @from, DateTime to, int take, int skip)
        {
            return InMemoryJobStorageMemoryStore.MemoryStore.Where
                (x => x.Value.TimeToRun>=@from && x.Value.TimeToRun<= to).Take(take).Skip(skip).Select(x => x.Key).ToList();
        }

        public List<string> LoadJobsByJobStateTypeAndTimeToRun(JobStateType stateType, DateTime @from, DateTime to, int take, int skip)
        {
            return InMemoryJobStorageMemoryStore.MemoryStore
                .Where(x =>
                x.Value.TimeToRun >= @from
                && x.Value.TimeToRun <= to &&
                x.Value.JobStateType == stateType)
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
            InMemoryJobStorageMemoryStore.MemoryStore = new ConcurrentDictionary<string, JobLoad>();
        }
    }
}