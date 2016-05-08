using System;
using System.Collections.Generic;
using System.Linq;
using Superpose.StorageInterface;
using SuperposeLib.Extensions;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Models;

namespace SuperposeLib.Core
{
    public class JobStateTransitionFactory
    {
        public IJobLoad GetNextState(IJobLoad jobLoad, SuperVisionDecision superVisionDecision)
        {
            IJobState nextState = new JobState
            {
                Started = DateTime.Now,
                PreviousJobExecutionStatusList = new List<JobExecutionStatus>()
            };
            if (jobLoad == null)
            {
                nextState = new JobState
                {
                    JobStateTypeName = Enum.GetName(typeof (JobStateType), JobStateType.Unknown),
                    PreviousJobExecutionStatusList = new List<JobExecutionStatus>()
                };
            }

            if (jobLoad != null)
            {
                jobLoad.PreviousJobExecutionStatusList = jobLoad.PreviousJobExecutionStatusList ??
                                                         new List<JobExecutionStatus>();
                nextState.PreviousJobExecutionStatusList.AddRange(jobLoad.PreviousJobExecutionStatusList);

                {
                    if (jobLoad.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Deleted))
                    {
                        nextState = jobLoad;
                    }

                    if (jobLoad.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Unknown))
                    {
                        nextState.JobStateTypeName = JobStateType.Queued.GetJobStateTypeName();
                    }

                    if (jobLoad.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Queued))
                    {
                        if (superVisionDecision == SuperVisionDecision.Fail)
                        {
                            nextState.JobStateTypeName = Enum.GetName(typeof (JobStateType), JobStateType.Failed);
                        }

                        if (superVisionDecision != SuperVisionDecision.Fail)
                        {
                            nextState.JobStateTypeName = Enum.GetName(typeof (JobStateType), JobStateType.Processing);
                        }
                    }

                    if (jobLoad.JobStateTypeName == Enum.GetName(typeof (JobStateType), JobStateType.Processing))
                    {
                        if (jobLoad.PreviousJobExecutionStatusList.LastOrDefault() == JobExecutionStatus.Unknown)
                        {
                            nextState.JobStateTypeName = JobStateType.Queued.GetJobStateTypeName();
                        }
                        if (jobLoad.PreviousJobExecutionStatusList.LastOrDefault() == JobExecutionStatus.Passed)
                        {
                            nextState.JobStateTypeName = JobStateType.Successfull.GetJobStateTypeName();
                        }
                        if (jobLoad.PreviousJobExecutionStatusList.LastOrDefault() == JobExecutionStatus.Failed)
                        {
                            if (superVisionDecision == SuperVisionDecision.Fail)
                            {
                                nextState.JobStateTypeName = Enum.GetName(typeof (JobStateType), JobStateType.Failed);
                            }
                            if (superVisionDecision == SuperVisionDecision.Pass)
                            {
                                nextState.JobStateTypeName = Enum.GetName(typeof (JobStateType),
                                    JobStateType.Successfull);
                            }
                            if (superVisionDecision != SuperVisionDecision.Fail &&
                                superVisionDecision != SuperVisionDecision.Pass)
                            {
                                nextState.JobStateTypeName = JobStateType.Queued.GetJobStateTypeName();
                            }
                        }
                    }
                }
            }
            jobLoad = UpdateJobLoadState(jobLoad, nextState);

            return jobLoad;
        }

        private static IJobLoad UpdateJobLoadState(IJobLoad jobLoad, IJobState nextState)
        {
            jobLoad = jobLoad ?? new JobLoad();
            jobLoad.PreviousJobExecutionStatusList = nextState.PreviousJobExecutionStatusList;
            jobLoad.Started = nextState.Started;
            jobLoad.Ended = nextState.Ended;
            jobLoad.JobStateTypeName = nextState.JobStateTypeName;
            return jobLoad;
        }
    }
}