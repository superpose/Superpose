using System;
using SuperposeLib.Interfaces.Storage;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Newtonsoft.Json;
using SuperposeLib.Models;

namespace Superpose.Storage.LiteDB
{
    public class LiteDBJobLoader : IJobLoader
    {

        private LiteCollection<JobLoadCollectionEntity> GetJobLoadCollection()
        {
              var  jobLoadCollection=  DB.GetCollection<JobLoadCollectionEntity>("JobLoad");
            return jobLoadCollection;
        }

        public LiteDBJobLoader()
        {
            DB = new LiteDatabase(@"MyData.db");
        }

      

        public LiteDatabase DB { get; set; }


        public void Dispose()
        {
          DB.Dispose();
        }

        public string LoadJobById(string jobId)
        {
         var collection=   GetJobLoadCollection().Find(x=>x.Id==jobId);
            var result = collection.Select(x => x.JobLoad).First();
            return result == null ? null : JsonConvert.SerializeObject(result);
        }

        public JobStatistics GetJobStatistics()
        {
            return new JobStatistics()
            {
                TotalNumberOfJobs = GetJobLoadCollection().Count(),
                TotalQueuedJobs = GetJobLoadCollection().Find(x => x.JobLoad.JobStateType == JobStateType.Queued).Count(),
                TotalProcessingJobs = GetJobLoadCollection().Find(x => x.JobLoad.JobStateType == JobStateType.Processing).Count(),
                TotalDeletedJobs = GetJobLoadCollection().Find(x => x.JobLoad.JobStateType == JobStateType.Deleted).Count(),
                TotalSuccessfullJobs = GetJobLoadCollection().Find(x => x.JobLoad.JobStateType == JobStateType.Successfull).Count(),
                TotalFailedJobs = GetJobLoadCollection().Find(x => x.JobLoad.JobStateType == JobStateType.Failed).Count(),
                TotalUnknownJobs = GetJobLoadCollection().Find(x => x.JobLoad.JobStateType == JobStateType.Unknown).Count()

            };
        }

        public List<string> LoadJobsByJobType(Type jobType, int take, int skip)
        {
            var collection = GetJobLoadCollection()
                .Find(x => x.JobLoad.JobType == jobType).Take(take).Skip(skip).Select(x => x.Id).ToList();
            return collection;
        }

        public List<string> LoadJobsByJobStateType(JobStateType stateType, int take, int skip)
        {
            var collection = GetJobLoadCollection()
             .Find(x => x.JobLoad.JobStateType == stateType).Take(take).Skip(skip).Select(x => x.Id).ToList();
            return collection;
        }

        public List<string> LoadJobsByTimeToRun(DateTime @from, DateTime to, int take, int skip)
        {
            var collection = GetJobLoadCollection()
             .Find(x => x.JobLoad.TimeToRun >= @from && x.JobLoad.TimeToRun <= to).Take(take).Skip(skip).Select(x => x.Id).ToList();
            return collection;
        }

        public List<string> LoadJobsByJobStateTypeAndTimeToRun(JobStateType stateType, DateTime @from, DateTime to, int take, int skip)
        {
            var collection = GetJobLoadCollection()
             .Find(x => x.JobLoad.JobStateType==stateType&& x.JobLoad.TimeToRun >= @from && x.JobLoad.TimeToRun <= to).Take(take).Skip(skip).Select(x => x.Id).ToList();
            return collection;
        }

        public List<string> LoadJobsByIds(List<string> ids)
        {
            return ids.Select(LoadJobById).ToList();
        }
    }
}