using SuperposeLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.JobThings;

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
                nextState = new JobState { JobStateType = JobStateType.Unknown, PreviousJobExecutionStatusList = new List<JobExecutionStatus>() };
            }

            if (jobLoad != null)
            {
                jobLoad.PreviousJobExecutionStatusList = jobLoad.PreviousJobExecutionStatusList ?? new List<JobExecutionStatus>();
                nextState.PreviousJobExecutionStatusList.AddRange(jobLoad.PreviousJobExecutionStatusList);

                {
                    if (jobLoad.JobStateType == JobStateType.Deleted)
                    {
                        nextState = jobLoad;
                    }

                    if (jobLoad.JobStateType == JobStateType.Unknown)
                    {
                        nextState.JobStateType = JobStateType.Queued;
                    }

                    if (jobLoad.JobStateType == JobStateType.Queued)
                    {
                        if (superVisionDecision == SuperVisionDecision.Fail)
                        {
                            nextState.JobStateType = JobStateType.Failed;
                        }

                        if (superVisionDecision != SuperVisionDecision.Fail)
                        {
                            nextState.JobStateType = JobStateType.Processing;
                        }
                    }

                    if (jobLoad.JobStateType == JobStateType.Processing)
                    {
                        if (jobLoad.PreviousJobExecutionStatusList.LastOrDefault() == JobExecutionStatus.Unknown)
                        {
                            nextState.JobStateType = JobStateType.Queued;
                        }
                        if (jobLoad.PreviousJobExecutionStatusList.LastOrDefault() == JobExecutionStatus.Passed)
                        {
                            nextState.JobStateType = JobStateType.Successfull;
                        }
                        if (jobLoad.PreviousJobExecutionStatusList.LastOrDefault() == JobExecutionStatus.Failed)
                        {
                            if (superVisionDecision == SuperVisionDecision.Fail)
                            {
                                nextState.JobStateType = JobStateType.Failed;
                            }
                            if (superVisionDecision == SuperVisionDecision.Pass)
                            {
                                nextState.JobStateType = JobStateType.Successfull;
                            }
                            if (superVisionDecision != SuperVisionDecision.Fail && superVisionDecision != SuperVisionDecision.Pass)
                            {
                                nextState.JobStateType = JobStateType.Queued;
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
            jobLoad.JobStateType = nextState.JobStateType;
            return jobLoad;
        }
    }
}