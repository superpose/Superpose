using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Superpose.StorageInterface;

namespace Superpose.Storage.SqlServer
{
    public class SqlServerJobLoader : IJobLoader
    {
        private readonly string Deleted = Enum.GetName(typeof (JobStateType), JobStateType.Deleted);
        private readonly string Failed = Enum.GetName(typeof (JobStateType), JobStateType.Failed);
        private readonly string Processing = Enum.GetName(typeof (JobStateType), JobStateType.Processing);
        private readonly string Queued = Enum.GetName(typeof (JobStateType), JobStateType.Queued);
        private readonly string Successfull = Enum.GetName(typeof (JobStateType), JobStateType.Successfull);
        private readonly string Unknown = Enum.GetName(typeof (JobStateType), JobStateType.Unknown);


        public void Dispose()
        {
        }

        //Db.JobLoads

        public string LoadJobById(string jobId)
        {
            using (var Db = new SuperPoseContext())
            {
                var data = Db.JobLoads.First(x => x.Id == jobId);
                return data == null ? null : JsonConvert.SerializeObject(data);
            }
        }

        public JobStatistics GetJobStatistics()
        {
            using (var Db = new SuperPoseContext())
            {
                return new JobStatistics
                {
                    TotalNumberOfJobs = Db.JobLoads.Count(),
                    TotalQueuedJobs =
                        Db.JobLoads.Count(
                            x => x.JobStateTypeName == Queued),
                    TotalProcessingJobs =
                        Db.JobLoads.Count(
                            x => x.JobStateTypeName == Processing),
                    TotalDeletedJobs =
                        Db.JobLoads.Count(
                            x => x.JobStateTypeName == Deleted),
                    TotalSuccessfullJobs =
                        Db.JobLoads.Count(
                            x => x.JobStateTypeName == Successfull),
                    TotalFailedJobs =
                        Db.JobLoads.Count(
                            x => x.JobStateTypeName == Failed),
                    TotalUnknownJobs =
                        Db.JobLoads.Count(
                            x => x.JobStateTypeName == Unknown)
                };
            }
        }

        public List<string> LoadJobIdsByJobType(string queueName, Type jobType, int take, int skip)
        {
            using (var Db = new SuperPoseContext())
            {
                return
                    Db.JobLoads.Where(
                        x => x.JobTypeFullName == jobType.AssemblyQualifiedName && x.JobQueueName == queueName)
                        .Take(take)
                        
                        .OrderBy(x=>x.Id).Skip(skip)
                        .Select(x => x.Id)
                        .ToList();
            }
        }

        public List<string> LoadJobIdsByJobStateType(string queueName, JobStateType stateType, int take, int skip)
        {
            var typeName = Enum.GetName(typeof (JobStateType), stateType);
            using (var Db = new SuperPoseContext())
            {
                return
                    Db.JobLoads.Where(
                        x =>
                            x.JobStateTypeName == typeName &&
                            x.JobQueueName == queueName)
                        .Take(take)
                        .OrderBy(x=>x.Id).Skip(skip)
                        .Select(x => x.Id)
                        .ToList();
            }
        }

        public List<SerializableJobLoad> LoadJobsByJobStateTypeAndQueue(string queueName, JobStateType stateType,
            int take, int skip)
        {
            var typeName = Enum.GetName(typeof (JobStateType), stateType);
            using (var Db = new SuperPoseContext())
            {
                return
                    Db.JobLoads.Where(
                        x =>
                            x.JobStateTypeName == typeName &&
                            x.JobQueueName == queueName)
                        .Take(take)
                        .OrderBy(x=>x.Id).Skip(skip)
                        .Select(x => x)
                        .ToList();
            }
        }

        public List<SerializableJobLoad> LoadJobsByQueue(string queueName, int take, int skip)
        {
            using (var Db = new SuperPoseContext())
            {
                return
                    Db.JobLoads.Where(
                        x =>
                            x.JobQueueName == queueName)
                        .Take(take)
                        .OrderBy(x=>x.Id).Skip(skip)
                        .Select(x => x)
                        .ToList();
            }
        }

        public List<SerializableJobLoad> LoadJobs(int take, int skip)
        {
            using (var Db = new SuperPoseContext())
            {
                return
                    Db.JobLoads
                        .Take(take)
                        .OrderBy(x=>x.Id).Skip(skip)
                        .Select(x => x)
                        .ToList();
            }
        }


        public List<string> LoadJobIdsByTimeToRun(string queueName, DateTime @from, DateTime to, int take, int skip)
        {
            using (var Db = new SuperPoseContext())
            {
                return Db.JobLoads.Where
                    (x => x.TimeToRun >= @from && x.TimeToRun <= to && x.JobQueueName == queueName)
                    .Take(take)
                    .OrderBy(x=>x.Id).Skip(skip)
                    .Select(x => x.Id)
                    .ToList();
            }
        }

        public List<string> LoadJobIdsByJobStateTypeAndTimeToRun(string queueName, JobStateType stateType,
            DateTime @from,
            DateTime to,
            int take, int skip)
        {
            var typeName = Enum.GetName(typeof (JobStateType), stateType);
            using (var Db = new SuperPoseContext())
            {
                return Db.JobLoads
                    .Where(x =>
                        x.TimeToRun >= @from
                        && x.TimeToRun <= to &&
                        x.JobStateTypeName == typeName &&
                        x.JobQueueName == queueName)
                    .OrderBy(x=>x.Id).Skip(skip)
                    .Take(take)
                    .Select(x => x.Id)
                    .ToList();
            }
        }

        public List<string> LoadJobsByIds(List<string> ids)
        {
            return ids.Select(LoadJobById).ToList();
        }
    }
}