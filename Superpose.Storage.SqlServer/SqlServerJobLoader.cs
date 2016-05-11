

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Superpose.StorageInterface;

namespace Superpose.Storage.SqlServer
{
    public class SqlServerJobLoader : IJobLoader
    {
        public SqlServerJobLoader()
        {
            Db = new SuperPoseContext();
        }

        public SuperPoseContext Db { get; set; }

        public void Dispose()
        {
           Db.Dispose();
        }
        //Db.JobLoads

        public string LoadJobById(string jobId)
        {
            SerializableJobLoad data = Db.JobLoads.First(x => x.Id == jobId);
            return data == null ? null : JsonConvert.SerializeObject(data);
        }

        public JobStatistics GetJobStatistics()
        {
            return new JobStatistics
            {
                TotalNumberOfJobs = Db.JobLoads.Count(),
                TotalQueuedJobs =
                     Db.JobLoads.Count(
                        x => x.JobStateTypeName == Enum.GetName(typeof(JobStateType), JobStateType.Queued)),
                TotalProcessingJobs =
                     Db.JobLoads.Count(
                        x => x.JobStateTypeName == Enum.GetName(typeof(JobStateType), JobStateType.Processing)),
                TotalDeletedJobs =
                     Db.JobLoads.Count(
                        x => x.JobStateTypeName == Enum.GetName(typeof(JobStateType), JobStateType.Deleted)),
                TotalSuccessfullJobs =
                     Db.JobLoads.Count(
                        x => x.JobStateTypeName == Enum.GetName(typeof(JobStateType), JobStateType.Successfull)),
                TotalFailedJobs =
                     Db.JobLoads.Count(
                        x => x.JobStateTypeName == Enum.GetName(typeof(JobStateType), JobStateType.Failed)),
                TotalUnknownJobs =
                     Db.JobLoads.Count(
                        x => x.JobStateTypeName == Enum.GetName(typeof(JobStateType), JobStateType.Unknown))
            };
        }

        public List<string> LoadJobIdsByJobType(string queueName, Type jobType, int take, int skip)
        {
            return
                    Db.JobLoads.Where(
                    x => x.JobTypeFullName == jobType.AssemblyQualifiedName && x.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Id)
                    .ToList();
        }

        public List<string> LoadJobIdsByJobStateType(string queueName, JobStateType stateType, int take, int skip)
        {
            return
                    Db.JobLoads.Where(
                    x =>
                        x.JobStateTypeName == Enum.GetName(typeof(JobStateType), stateType) &&
                        x.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Id)
                    .ToList();
        }
        public List<SerializableJobLoad> LoadJobsByJobStateTypeAndQueue(string queueName, JobStateType stateType, int take, int skip)
        {
            return
                    Db.JobLoads.Where(
                    x =>
                        x.JobStateTypeName == Enum.GetName(typeof(JobStateType), stateType) &&
                        x.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x)
                    .ToList();
        }

        public List<SerializableJobLoad> LoadJobsByQueue(string queueName, int take, int skip)
        {
            return
                    Db.JobLoads.Where(
                    x =>
                    x.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x)
                    .ToList();
        }

        public List<SerializableJobLoad> LoadJobs(int take, int skip)
        {
            return
                    Db.JobLoads
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x)
                    .ToList();
        }



        public List<string> LoadJobIdsByTimeToRun(string queueName, DateTime @from, DateTime to, int take, int skip)
        {
            return     Db.JobLoads.Where
                (x => x.TimeToRun >= @from && x.TimeToRun <= to && x.JobQueueName == queueName)
                .Take(take)
                .Skip(skip)
                .Select(x => x.Id)
                .ToList();
        }

        public List<string> LoadJobIdsByJobStateTypeAndTimeToRun(string queueName, JobStateType stateType, DateTime @from,
            DateTime to,
            int take, int skip)
        {
            return     Db.JobLoads
                .Where(x =>
                    x.TimeToRun >= @from
                    && x.TimeToRun <= to &&
                    x.JobStateTypeName == Enum.GetName(typeof(JobStateType), stateType) &&
                    x.JobQueueName == queueName)
                .Skip(skip)
                .Take(take)
                .Select(x => x.Id)
                .ToList();
        }

        public List<string> LoadJobsByIds(List<string> ids)
        {
            return ids.Select(LoadJobById).ToList();
        }
    }
}