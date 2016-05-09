using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Superpose.StorageInterface;

namespace Superpose.Storage.LiteDB
{
    public class LiteDbJobLoader : IJobLoader
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string LoadJobById(string jobId)
        {
            throw new NotImplementedException();
        }

        public JobStatistics GetJobStatistics()
        {
            throw new NotImplementedException();
        }

        public List<string> LoadJobIdsByJobType(string queueName, Type jobType, int take, int skip)
        {
            throw new NotImplementedException();
        }

        public List<string> LoadJobIdsByJobStateType(string queueName, JobStateType stateType, int take, int skip)
        {
            throw new NotImplementedException();
        }

        public List<string> LoadJobIdsByTimeToRun(string queueName, DateTime @from, DateTime to, int take, int skip)
        {
            throw new NotImplementedException();
        }

        public List<string> LoadJobIdsByJobStateTypeAndTimeToRun(string queueName, JobStateType stateType, DateTime @from, DateTime to,
            int take, int skip)
        {
            throw new NotImplementedException();
        }

        public List<string> LoadJobsByIds(List<string> ids)
        {
            throw new NotImplementedException();
        }

        public List<SerializableJobLoad> LoadJobsByJobStateType(string queueName, JobStateType stateType, int take, int skip)
        {
            throw new NotImplementedException();
        }

        /*
         
         public string LoadJobById(string jobId)
        {
            SerializableJobLoad result = null;
            LiteDbCollectionsFactory.UseLiteDatabase(jobLoadCollection =>
            {
                var collection = jobLoadCollection.Find(x => x.Id == jobId);

                result = collection.FirstOrDefault();
            });
            return result == null ? null : JsonConvert.SerializeObject(result);
        }

        public JobStatistics GetJobStatistics()
        {
            JobStatistics result = null;
            LiteDbCollectionsFactory.UseLiteDatabase(jobLoadCollection =>
            {
                result = new JobStatistics
                {
                    TotalNumberOfJobs = jobLoadCollection.Count(),
                    TotalQueuedJobs =
                        jobLoadCollection.Find(
                            x => x.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Queued)).Count(),
                    TotalProcessingJobs =
                        jobLoadCollection.Find(
                            x => x.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Processing))
                            .Count(),
                    TotalDeletedJobs =
                        jobLoadCollection.Find(
                            x => x.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Deleted))
                            .Count(),
                    TotalSuccessfullJobs =
                        jobLoadCollection.Find(
                            x => x.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Successfull))
                            .Count(),
                    TotalFailedJobs =
                        jobLoadCollection.Find(
                            x => x.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Failed)).Count(),
                    TotalUnknownJobs =
                        jobLoadCollection.Find(
                            x => x.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Unknown))
                            .Count()
                };
            });
            return result;
        }

        public List<string> LoadJobsByJobType(string queueName, Type jobType, int take, int skip)
        {
            List<string> collection = null;
            LiteDbCollectionsFactory.UseLiteDatabase(jobLoadCollection =>
            {
                collection = jobLoadCollection
                    .Find(x => x.JobTypeFullName == jobType.AssemblyQualifiedName && x.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Id)
                    .ToList();
            });

            return collection;
        }

        public List<string> LoadJobIdsByJobStateType(string queueName, JobStateType stateType, int take, int skip)
        {
            List<string> collection = null;
            LiteDbCollectionsFactory.UseLiteDatabase(jobLoadCollection =>
            {
                collection = jobLoadCollection
                    .Find(
                        x =>
                            x.JobStateTypeName == Enum.GetName(typeof (JobStateType), stateType) &&
                            x.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Id)
                    .ToList();
            });

            return collection;
        }

        public List<SerializableJobLoad> LoadJobsByJobStateType(string queueName, JobStateType stateType, int take, int skip)
        {
            List<SerializableJobLoad> collection = null;
            LiteDbCollectionsFactory.UseLiteDatabase(jobLoadCollection =>
            {
                collection = jobLoadCollection
                    .Find(
                        x =>
                            x.JobStateTypeName == Enum.GetName(typeof(JobStateType), stateType) &&
                            x.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .ToList();
            });

            return collection;
        }

        public List<string> LoadJobsByTimeToRun(string queueName, DateTime @from, DateTime to, int take, int skip)
        {
            List<string> collection = null;
            LiteDbCollectionsFactory.UseLiteDatabase(jobLoadCollection =>
            {
                collection = jobLoadCollection
                    .Find(x => x.TimeToRun >= @from && x.TimeToRun <= to && x.JobQueueName == queueName)
                    .Take(take)
                    .Skip(skip)
                    .Select(x => x.Id)
                    .ToList();
            });
            return collection;
        }

        public List<string> LoadJobsByJobStateTypeAndTimeToRun(string queueName, JobStateType stateType, DateTime @from,
            DateTime to,
            int take, int skip)
        {
            List<string> collection = null;
            LiteDbCollectionsFactory.UseLiteDatabase(jobLoadCollection =>
            {
                var collectionTmp = jobLoadCollection
                    .Find(
                        x =>
                            x.JobStateTypeName == Enum.GetName(typeof (JobStateType), stateType) && x.TimeToRun >= @from &&
                            x.TimeToRun <= to && x.JobQueueName == queueName);

                collection = collectionTmp.Select(x => x.Id).Take(take).Skip(skip).ToList();
            });
            return collection;
        }

        public List<string> LoadJobsByIds(List<string> ids)
        {
            return ids.Select(LoadJobById).ToList();
        }

        public void Dispose()
        {
        }
         */
    }
}