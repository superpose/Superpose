using SuperposeLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperposeLib.Core
{
    public class JobStateTransitionFactory
    {
        public JobState GetNextState(JobState currentState, SuperVisionDecision superVisionDecision)
        {
            var nextState = new JobState
            {
                Started = DateTime.Now,
                PreviousJobStatus = new List<JobStatus>()
            };
            if (currentState == null)
            {
                nextState = new JobState { JobStateType = JobStateType.Unknown, PreviousJobStatus = new List<JobStatus>() };
            }

            if (currentState != null)
            {
                currentState.PreviousJobStatus = currentState.PreviousJobStatus ?? new List<JobStatus>();
                nextState.PreviousJobStatus.AddRange(currentState.PreviousJobStatus);

                {
                    if (currentState.JobStateType == JobStateType.Deleted)
                    {
                        nextState = currentState;
                    }

                    if (currentState.JobStateType == JobStateType.Unknown)
                    {
                        nextState.JobStateType = JobStateType.Queued;
                    }

                    if (currentState.JobStateType == JobStateType.Queued)
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

                    if (currentState.JobStateType == JobStateType.Processing)
                    {
                        if (currentState.PreviousJobStatus.LastOrDefault() == JobStatus.Unknown)
                        {
                            nextState.JobStateType = JobStateType.Queued;
                        }
                        if (currentState.PreviousJobStatus.LastOrDefault() == JobStatus.Passed)
                        {
                            nextState.JobStateType = JobStateType.Successfull;
                        }
                        if (currentState.PreviousJobStatus.LastOrDefault() == JobStatus.Failed)
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
            return nextState;
        }
    }
}