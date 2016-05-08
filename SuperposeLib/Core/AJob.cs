using System;
using Superpose.StorageInterface;
using SuperposeLib.Models;

namespace SuperposeLib.Core
{
    internal class PrivateJob : AJob<DefaultAJobCommand>
    {
        protected override void Execute(DefaultAJobCommand command)
        {
            throw new NotImplementedException();
        }
    }


    public abstract class AJob : AJob<DefaultAJobCommand>
    {
        protected override void Execute(DefaultAJobCommand command = null)
        {
            Execute();
        }

        protected abstract void Execute();
    }

    public abstract class AJob<T> where T : AJobCommand
    {
        protected abstract void Execute(T command = default(T));

        public virtual SuperVisionDecision Supervision(Exception reaon, int totalNumberOfHistoricFailures)
        {
            return totalNumberOfHistoricFailures > 10 ? SuperVisionDecision.Fail : SuperVisionDecision.ReQueue;
        }

        public JobResult RunJob(T command)
        {
            var result = new JobResult();
            try
            {
                Execute(command);
                result.IsSuccessfull = true;
            }
            catch (Exception e)
            {
                result.Exception = e;
            }
            return result;
        }
    }
}