﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Core.Jobs;
using SuperposeLib.Extensions;
using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Models;

namespace SuperposeLib.Core
{
    public class JobFactory : IJobFactory
    {
        public JobFactory(IJobStorage jobStorage, IJobConverter jobConverter, ITime time = null)
        {
            time = time ?? new RealTime();
            Time = time;
            if (jobStorage == null) throw new ArgumentNullException(nameof(jobStorage));
            if (jobConverter == null) throw new ArgumentNullException(nameof(jobConverter));
            JobConverter = jobConverter;
            JobStorage = jobStorage;

            MaxWaitSecondsBeforeOverridingCurrentProcessingJob = 2*60;
        }
        public int MaxWaitSecondsBeforeOverridingCurrentProcessingJob { set; get; }
        public IJobConverter JobConverter { set; get; }
        public ITime Time { set; get; }
        public IJobStorage JobStorage { get; set; }
        public string QueueJob<T>(AJobCommand command = null, JobQueue jobQueue = null)
        {
            var jobType = typeof (T);
            return ScheduleJob(jobType, command, Time.UtcNow, jobQueue);
        }
        public string QueueJob(Type jobType, AJobCommand command = null, JobQueue jobQueue = null)
        {
            return ScheduleJob(jobType, command, Time.UtcNow, jobQueue);
        }
        public string ScheduleJob<T>(AJobCommand command = null, DateTime? scheduleTime = null,JobQueue jobQueue = null)
        {
            var jobType = typeof (T);
            return ScheduleJob(jobType, command, scheduleTime, jobQueue);
        }
        public string ScheduleJob(Type jobType, AJobCommand command = null, DateTime? scheduleTime = null,JobQueue jobQueue = null)
        {
            var jobLoad = PrepareScheduleJob(jobType, command, scheduleTime, jobQueue);
            JobStorage.JobSaver.SaveNew(JobConverter.SerializeJobLoad(jobLoad), jobLoad.Id);
            return jobLoad.Id;
        }

        public JobLoad PrepareScheduleJob(Type jobType, AJobCommand command = null, DateTime? scheduleTime = null, JobQueue jobQueue = null)
        {
            jobQueue = jobQueue ?? new DefaultJobQueue();
            var jobId = Guid.NewGuid().ToString();
            var jobLoad = new JobLoad
            {
                JobCommandTypeFullName = command?.GetType().AssemblyQualifiedName,
                Command = JobConverter.SerializeJobCommand(command),
                JobQueue = jobQueue,
                JobQueueName = jobQueue.GetType().Name,
                TimeToRun = scheduleTime,
                JobTypeFullName = jobType.AssemblyQualifiedName,
                Id = jobId,
                JobStateTypeName = Enum.GetName(typeof (JobStateType), JobStateType.Unknown)
            };
            jobLoad = (JobLoad)  JobStateTransitionFactory.GetNextState(jobLoad, SuperVisionDecision.Unknown);
            return jobLoad;
        }

        public static string RunMethodName { set; get; }
       

        static string GetRunJobMethodName() 
        {
            if (RunMethodName != null) return RunMethodName;
            Expression<Action<PrivateJob>> action=(p) => p.RunJob(null);
            var methodCallExpression = action.Body as MethodCallExpression;
            if (methodCallExpression != null)
                RunMethodName=methodCallExpression.Method.Name;
            return RunMethodName;
        }
        public JobLoad InstantiateJobComponent(IJobLoad jobLoad)
        {
            var load = (JobLoad)jobLoad;
            try
            {
                var job = (AJob)Activator.CreateInstance(Type.GetType(jobLoad.JobTypeFullName));

                load.Job = job;
            }
            catch (Exception e)
            {
                load.Job = new CoreJobThatFails(e, jobLoad.JobTypeFullName);
            }
            return load;
        }

        public JobResult InstantiateAndRunJob(IJobLoad jobLoad)
        {
            var result=new JobResult();
            try
            {
                if (jobLoad?.JobTypeFullName != null)
                {
                    var type = Type.GetType(jobLoad.JobTypeFullName);
                    var commandType = string.IsNullOrEmpty(jobLoad.JobCommandTypeFullName)?null: Type.GetType(jobLoad.JobCommandTypeFullName);
                    if (type != null)
                    {
                        var method = type.GetMethod(GetRunJobMethodName());
                        if (commandType == null)
                        {
                            result = (JobResult)method.Invoke(Activator.CreateInstance(type), new[] { new DefaultAJobCommand(),  });
                        }
                        else
                        {
                       var command =  JsonConvert.DeserializeObject(jobLoad.Command, commandType);
                        result = (JobResult) method.Invoke(Activator.CreateInstance(type), new[] { command });
                   
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var Job = new CoreJobThatFails(e, jobLoad?.JobTypeFullName);
                 result = Job.RunJob(null);
            }
            return result;
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
                jobLoad = (JobLoad) JobConverter.Parse(data);

                if (jobLoad == null)
                {
                    throw new Exception("Unable to create jobLoad instance from raw job data : jobId - " + jobId + " : " +
                                        data);
                }

                if (jobLoad.JobTypeFullName == null)
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

            if (!IsEligibleToRun(jobLoad, Time))
                return null;

            if (!TryUpdateStorageBeforeJobExecutuinStarts(ref jobLoad))
                return null;

            var result  = InstantiateAndRunJob(jobLoad);

            if (!TryUpdateStorageAfterJobExecutionEnds(result, ref jobLoad))
                return null;

            return jobLoad;
        }
        private bool TryUpdateStorageAfterJobExecutionEnds(JobResult result, ref JobLoad jobLoad)
        {
            jobLoad.PreviousJobExecutionStatusList.Add(result.IsSuccessfull
                ? JobExecutionStatus.Passed
                : JobExecutionStatus.Failed);

            if (!result.IsSuccessfull)
            {
                try
                {
                    jobLoad.Job = InstantiateJobComponent(jobLoad).Job;
                    result.SuperVisionDecision = jobLoad.Job.Supervision(result.Exception,
                        jobLoad.HistoricFailureCount());
                }
                catch (Exception se)
                {
                    result.SuperVisionDecision = SuperVisionDecision.Fail;
                    result.SuperVisionException = se;
                }
            }
            jobLoad = (JobLoad)  JobStateTransitionFactory.GetNextState(jobLoad, result.SuperVisionDecision);

            var canUpdate = false;
            try
            {
                jobLoad.Job = null;
                JobStorage.JobSaver.Update(JobConverter.SerializeJobLoad(jobLoad), jobLoad.Id);
                canUpdate = true;
            }
            catch (Exception e)
            {
                canUpdate = false;
            }
            return canUpdate;
        }
        private bool TryUpdateStorageBeforeJobExecutuinStarts(ref JobLoad jobLoad)
        {
            jobLoad.JobStateTypeName = JobStateType.Queued.GetJobStateTypeName();
            jobLoad.Started = Time.UtcNow;

            jobLoad = (JobLoad)  JobStateTransitionFactory.GetNextState(jobLoad, SuperVisionDecision.Unknown);

            var updateStorageToProcessing = false;
            try
            {
                JobStorage.JobSaver.Update(JobConverter.SerializeJobLoad(jobLoad), jobLoad.Id);
                updateStorageToProcessing = true;
            }
            catch (Exception e)
            {
            }
            return updateStorageToProcessing;
        }
        private bool IsEligibleToRun(IJobLoad jobLoad, ITime time)
        {
            if (jobLoad == null)
                return false;

            var aboutTime = jobLoad.TimeToRun != null && time.UtcNow >= jobLoad.TimeToRun.Value;
            var itsTimeToProcess = jobLoad.TimeToRun == null || aboutTime;

            var jobIsInQueue = jobLoad.JobStateTypeName == JobStateType.Queued.GetJobStateTypeName();

            var canOverideCurrentlyProcessing = jobLoad.JobStateTypeName ==
                                                Enum.GetName(typeof (JobStateType), JobStateType.Processing) &&
                                                (jobLoad.Started == null ||
                                                 ((time.UtcNow - jobLoad.Started.Value).TotalMinutes >
                                                  MaxWaitSecondsBeforeOverridingCurrentProcessingJob));

            var canProcess = itsTimeToProcess && (jobIsInQueue || canOverideCurrentlyProcessing);
            return canProcess;
        }
    }
}