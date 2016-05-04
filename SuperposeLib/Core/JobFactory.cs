using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;
using System;
using SuperposeLib.Extensions;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public class JobFactory : IJobFactory
    {
        public ITime Time { set; get; }
        public int MaxWaitSecondsBeforeOverridingCurrentProcessingJob { set; get; }
        public IJobConverter JobConverter { set; get; }
        public IJobStorage JobStorage { get; set; }
        public JobFactory(IJobStorage jobStorage, IJobConverter jobConverter, ITime time = null)
        {
            time = time ?? new RealTime();
            Time = time;
            if (jobStorage == null) throw new ArgumentNullException(nameof(jobStorage));
            if (jobConverter == null) throw new ArgumentNullException(nameof(jobConverter));
            JobConverter = jobConverter;
            JobStorage = jobStorage;

            MaxWaitSecondsBeforeOverridingCurrentProcessingJob = 2 * 60;
        }
        public string QueueJob(Type jobType)
        {
            return ScheduleJob(jobType, Time.UtcNow);
        }
        public string ScheduleJob(Type jobType, DateTime? scheduleTime)
        {
            var jobId = Guid.NewGuid().ToString();
            var jobLoad = new JobLoad() { TimeToRun = scheduleTime, JobType = jobType, JobId = jobId };
            jobLoad = (JobLoad)new JobStateTransitionFactory().GetNextState(jobLoad, SuperVisionDecision.Unknown);
            JobStorage.JobSaver.SaveNew(JobConverter.Serialize(jobLoad), jobId);
            return jobId;
        }
        public JobLoad InstantiateJobComponent(IJobLoad jobLoad)
        {
            var load = ((JobLoad)jobLoad);
            load.Job = (AJob)Activator.CreateInstance(jobLoad.JobType);
            return load;
        }
        public JobLoad GetJobLoad(string jobId)
        {

            JobLoad jobLoad;
            try
            {
                var data = JobStorage.JobLoader.LoadJobById(jobId);
                if (data == null)
                {
                    throw new Exception("Unable to load jobLoad :  jobId - " + jobId);
                }
                jobLoad = (JobLoad)JobConverter.Parse(data);

                if (jobLoad == null)
                {
                    throw new Exception("Unable to create jobLoad instance from raw job data : jobId - " + jobId + " : " + data);
                }

                if (jobLoad.JobType == null)
                {
                    throw new Exception("Unable to determine job type :  jobId -" + jobId + " : " + data);
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return jobLoad;
        }
        public IJobLoad ProcessJob(string jobId)
        {
            var jobLoad = GetJobLoad(jobId);

            if (jobLoad == null) return null;
            var now = Time.UtcNow;
            var aboutTime = jobLoad.TimeToRun != null && now >= jobLoad.TimeToRun.Value;
            var itsTimeToProcess = jobLoad.TimeToRun == null || aboutTime;

            var jobIsInQueue = jobLoad.JobStateType == JobStateType.Queued;

            var canOverideCurrentlyProcessing = jobLoad.JobStateType == JobStateType.Processing &&
                (jobLoad.Started == null || ((Time.UtcNow - jobLoad.Started.Value).TotalMinutes > MaxWaitSecondsBeforeOverridingCurrentProcessingJob));

            var canProcess = itsTimeToProcess && (jobIsInQueue || canOverideCurrentlyProcessing);

            if (!canProcess) return null;

            jobLoad.JobStateType = JobStateType.Queued;
            jobLoad.Started = Time.UtcNow;

            jobLoad = (JobLoad)new JobStateTransitionFactory().GetNextState(jobLoad, SuperVisionDecision.Unknown);

            //persist
            var updateStorageToProcessing = false;
            try
            {
                JobStorage.JobSaver.Update(JobConverter.Serialize(jobLoad), jobLoad.JobId);
                updateStorageToProcessing = true;
            }
            catch (Exception e)
            {
                //failed to update after load
                //throw e;
            }
            if (!updateStorageToProcessing) return null;

            jobLoad = InstantiateJobComponent(jobLoad);
            var result = jobLoad.Job.RunJob();
            jobLoad.PreviousJobExecutionStatusList.Add(result.IsSuccessfull ? JobExecutionStatus.Passed : JobExecutionStatus.Failed);

            if (!result.IsSuccessfull)
            {
                try
                {
                    result.SuperVisionDecision = jobLoad.Job.Supervision(result.Exception, jobLoad.HistoricFailureCount());
                }
                catch (Exception se)
                {
                    result.SuperVisionDecision = SuperVisionDecision.Fail;
                    result.SuperVisionException = se;
                }
            }


            jobLoad = (JobLoad)new JobStateTransitionFactory().GetNextState(jobLoad, result.SuperVisionDecision);

            try
            {
                jobLoad.Job = null;
                JobStorage.JobSaver.Update(JobConverter.Serialize(jobLoad), jobLoad.JobId);

                return jobLoad;
            }
            catch (Exception e)
            {
                //failed to update after processing
                // throw e;
            }
            return null;
        }
    }
}