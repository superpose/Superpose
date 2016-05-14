﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Core.ActorSystem;
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

        public static string RunMethodName { set; get; }
        public ITime Time { set; get; }
        public IJobStorage JobStorage { get; set; }

        public string QueueJob<T>(AJobCommand command = null, JobQueue jobQueue = null, List<string> nextJob = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            var jobType = typeof (T);
            return ScheduleJob(new JobScheduleContainer
            {
                JobType = jobType,
                Command = command,
                ScheduleTime = Time.UtcNow,
                JobQueue = jobQueue,
                NextJob = nextJob
            },  enqueueStrategy );
        }

        public string QueueJob(Type jobType, AJobCommand command = null, JobQueue jobQueue = null,
            List<string> nextJob = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return ScheduleJob(new JobScheduleContainer
            {
                JobType = jobType,
                Command = command,
                ScheduleTime = Time.UtcNow,
                JobQueue = jobQueue,
                NextJob = nextJob
            },enqueueStrategy); //(jobType, command, Time.UtcNow, jobQueue, nextJob);
        }

        public string ScheduleJob<T>(AJobCommand command = null, DateTime? scheduleTime = null, JobQueue jobQueue = null,
            List<string> nextJob = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            var jobType = typeof (T);
            return ScheduleJob(new JobScheduleContainer
            {
                JobType = jobType,
                Command = command,
                ScheduleTime = scheduleTime,
                JobQueue = jobQueue,
                NextJob = nextJob
            },enqueueStrategy);
        }

        public string ScheduleJob(Type type, AJobCommand command = null, DateTime? scheduleTime = null,
            JobQueue jobQueue = null, List<string> nextJob = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return ScheduleJob(new JobScheduleContainer
            {
                JobType = type,
                Command = command,
                ScheduleTime = scheduleTime,
                JobQueue = jobQueue,
                NextJob = nextJob
            },enqueueStrategy)
                ;
        }

        private static SlimActor<JobScheduleContainer, string> EnqueueActor {set;get; }

        public string ScheduleJob(JobScheduleContainer container,  EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            EnqueueActor = EnqueueActor ?? new SlimActor<JobScheduleContainer, string>(1);

            container.JobId =  Guid.NewGuid().ToString();

            if (enqueueStrategy == EnqueueStrategy.Unknown)
            {
                enqueueStrategy  = EnqueueStrategy.Cpu;
            }

            switch (enqueueStrategy)
            {
                case EnqueueStrategy.Cpu:
                    var result1 = PrepareScheduleJob(container.JobType, container.Command, container.ScheduleTime, container.JobQueue, container.NextJob);
                    JobStorage.JobSaver.SaveNew(result1.JobLoadString, result1.JobLoad.Id);

                    break;
                case EnqueueStrategy.Queue:

                    var task1 = EnqueueActor.Tell(container, c =>
                    {
                        var result = PrepareScheduleJob(c.JobType, c.Command, c.ScheduleTime, c.JobQueue, c.NextJob, container.JobId);
                        JobStorage.JobSaver.SaveNew(result.JobLoadString, result.JobLoad.Id);

                        return Task.FromResult(result.JobLoad.Id);
                    }, null);

                    Task.WaitAll(task1);
                    break;
                case EnqueueStrategy.QueueCpu:
                    var task = EnqueueActor.Ask(container, c =>
                    {
                        var result = PrepareScheduleJob(c.JobType, c.Command, c.ScheduleTime, c.JobQueue, c.NextJob, container.JobId);
                        JobStorage.JobSaver.SaveNew(result.JobLoadString, result.JobLoad.Id);

                        return Task.FromResult(result.JobLoad.Id);
                    }, null);

                    Task.WaitAll(task);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(enqueueStrategy), enqueueStrategy, null);
            }

          
            return container.JobId;
           
        }

      

        public SerializedJobLoad PrepareScheduleJob(Type jobType, AJobCommand command = null, DateTime? scheduleTime = null, JobQueue jobQueue = null, List<string> nextJob = null, string jobId = null)
        {
            nextJob = nextJob ?? new List<string>();
            jobQueue = jobQueue ?? new DefaultJobQueue();
            jobQueue.Id = Guid.NewGuid().ToString();
            jobId = string.IsNullOrEmpty(jobId) ? Guid.NewGuid().ToString() : jobId;
            var jobLoad = new JobLoad
            {
                JobCommandTypeFullName = command?.GetType().AssemblyQualifiedName, NextCommand = nextJob, Command = JobConverter.SerializeJobCommand(command), JobQueue = jobQueue, JobQueueName = jobQueue.GetType().Name, TimeToRun = scheduleTime ?? Time.UtcNow, JobTypeFullName = jobType.AssemblyQualifiedName, JobName = jobType.Name, Id = jobId, JobStateTypeName = Enum.GetName(typeof (JobStateType), JobStateType.Unknown), LastUpdated = Time.UtcNow, QueuedAt = Time.UtcNow, LastUpdatedOnServer = Environment.MachineName, QueuedOnServer = Environment.MachineName
            };
            jobLoad = (JobLoad) JobStateTransitionFactory.GetNextState(jobLoad, SuperVisionDecision.Unknown);
            return new SerializedJobLoad
            {
                JobLoadString = JobConverter.SerializeJobLoad(jobLoad), JobLoad = jobLoad
            };
        }

        public JobLoad InstantiateJobComponent(IJobLoad jobLoad)
        {
            var load = (JobLoad) jobLoad;
            try
            {
                var job = (AJob) Activator.CreateInstance(Type.GetType(jobLoad.JobTypeFullName));

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
            var result = new JobResult();
            try
            {
                if (jobLoad?.JobTypeFullName != null)
                {
                    var type = Type.GetType(jobLoad.JobTypeFullName);
                    var commandType = string.IsNullOrEmpty(jobLoad.JobCommandTypeFullName) ? null : Type.GetType(jobLoad.JobCommandTypeFullName);
                    if (type != null)
                    {
                        var method = type.GetMethod(GetRunJobMethodName());
                        if (commandType == null)
                        {
                            result = (JobResult) method.Invoke(Activator.CreateInstance(type), new[] {new DefaultAJobCommand()});
                        }
                        else
                        {
                            var command = JsonConvert.DeserializeObject(jobLoad.Command, commandType);
                            result = (JobResult) method.Invoke(Activator.CreateInstance(type), new[] {command});
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
                    throw new Exception("Unable to create jobLoad instance from raw job data : jobId - " + jobId + " : " + data);
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

            var result = InstantiateAndRunJob(jobLoad);

            if (!TryUpdateStorageAfterJobExecutionEnds(result, jobLoad))
                return null;


            var fdgdfgdg = GetJobLoad(jobId);

            return jobLoad;
        }


        private static string GetRunJobMethodName()
        {
            if (RunMethodName != null) return RunMethodName;
            Expression<Action<PrivateJob>> action = p => p.RunJob(null);
            var methodCallExpression = action.Body as MethodCallExpression;
            if (methodCallExpression != null)
                RunMethodName = methodCallExpression.Method.Name;
            return RunMethodName;
        }

        private bool TryUpdateStorageAfterJobExecutionEnds(JobResult result, JobLoad jobLoad)
        {
            jobLoad.PreviousJobExecutionStatusList.Add(result.IsSuccessfull ? JobExecutionStatus.Passed : JobExecutionStatus.Failed);

            if (!result.IsSuccessfull)
            {
                try
                {
                    jobLoad.Job = InstantiateJobComponent(jobLoad).Job;
                    result.SuperVisionDecision = jobLoad.Job.Supervision(result.Exception, jobLoad.HistoricFailureCount());
                }
                catch (Exception se)
                {
                    result.SuperVisionDecision = SuperVisionDecision.Fail;
                    result.SuperVisionException = se;
                }
            }
            jobLoad = (JobLoad) JobStateTransitionFactory.GetNextState(jobLoad, result.SuperVisionDecision);

            if (jobLoad.JobStateTypeName == JobStateType.Successfull.GetJobStateTypeName() && jobLoad.NextCommand != null && !string.IsNullOrEmpty(jobLoad.NextCommand.FirstOrDefault()))
            {
                try
                {
                    var head = jobLoad.NextCommand.FirstOrDefault();
                    var tail = jobLoad.NextCommand.Count > 1 ? jobLoad.NextCommand.Skip(1).Take(jobLoad.NextCommand.Count - 1).ToList() : null;

                    var nextJobLoad = JobConverter.JobParser.Execute(head);
                    if (!string.IsNullOrEmpty(nextJobLoad?.Id))
                    {
                        try
                        {
                            JobStorage.JobSaver.SaveNew(head, nextJobLoad.Id);
                            if (tail != null)
                            {
                                JobHandler.EnqueueJob<PilotJob>(continuation => tail);
                            }
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                    jobLoad.NextCommand = tail;
                }
                catch (Exception)
                {
                    // failed to proc continuation
                }
            }


            return FinallyUpdateCurrentJobStorage(jobLoad);
        }

        private bool FinallyUpdateCurrentJobStorage(JobLoad jobLoad)
        {
            bool canUpdate;
            try
            {
                jobLoad.Job = null;
                jobLoad.LastUpdated = Time.UtcNow;
                jobLoad.LastUpdatedOnServer = Environment.MachineName;

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

            jobLoad = (JobLoad) JobStateTransitionFactory.GetNextState(jobLoad, SuperVisionDecision.Unknown);

            var updateStorageToProcessing = false;
            try
            {
                jobLoad.LastUpdated = Time.UtcNow;
                jobLoad.LastUpdatedOnServer = Environment.MachineName;
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

            var canOverideCurrentlyProcessing = jobLoad.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Processing) && (jobLoad.Started == null || ((time.UtcNow - jobLoad.Started.Value).TotalMinutes > MaxWaitSecondsBeforeOverridingCurrentProcessingJob));

            var canProcess = itsTimeToProcess && (jobIsInQueue || canOverideCurrentlyProcessing);
            return canProcess;
        }
    }
}