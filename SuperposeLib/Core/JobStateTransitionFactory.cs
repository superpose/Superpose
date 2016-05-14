using System;
using System.Collections.Generic;
using System.Linq;
using Superpose.StorageInterface;
using SuperposeLib.Extensions;
using SuperposeLib.Models;

namespace SuperposeLib.Core
{
    public class JobStateTransitionFactory
    {
        public static IJobLoad GetNextState(IJobLoad jobLoad, SuperVisionDecision superVisionDecision)
        {
            IJobState nextState = new JobState
            {
                Started = DateTime.Now,
                PreviousJobExecutionStatusList = ""
            };
            if (jobLoad == null)
            {
                nextState = new JobState
                {
                    JobStateTypeName = Enum.GetName(typeof (JobStateType), JobStateType.Unknown),
                    PreviousJobExecutionStatusList = ""
                };
            }

            if (jobLoad != null)
            {
                jobLoad.PreviousJobExecutionStatusList = jobLoad.PreviousJobExecutionStatusList ??"";
                foreach (var jobExecutionStatuse in jobLoad.PreviousJobExecutionStatusList.Split(','))
                {
                    
                    if (string.IsNullOrEmpty(nextState.PreviousJobExecutionStatusList) || string.IsNullOrEmpty(jobExecutionStatuse))
                    {
                        nextState.PreviousJobExecutionStatusList = jobExecutionStatuse;
                    }
                    else
                    {
                        nextState.PreviousJobExecutionStatusList = nextState.PreviousJobExecutionStatusList+ "," + jobExecutionStatuse;
                    }

                }


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
                        if (string.IsNullOrEmpty(jobLoad.PreviousJobExecutionStatusList))
                        {
                            nextState.JobStateTypeName = JobStateType.Queued.GetJobStateTypeName();
                        }
                        if (jobLoad.PreviousJobExecutionStatusList.Split(',').LastOrDefault() == JobExecutionStatus.Passed.ToStringName())
                        {
                            nextState.JobStateTypeName = JobStateType.Successfull.GetJobStateTypeName();
                        }
                        if (jobLoad.PreviousJobExecutionStatusList.Split(',').LastOrDefault() == JobExecutionStatus.Failed.ToStringName())
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
            jobLoad.LastUpdated = nextState.LastUpdated;
            jobLoad.JobStateTypeName = nextState.JobStateTypeName;
            return jobLoad;
        }
    }
}