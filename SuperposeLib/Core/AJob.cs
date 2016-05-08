using System;
using Superpose.StorageInterface;
using SuperposeLib.Models;
using SuperposeLib.Services.DefaultConverter;

namespace SuperposeLib.Core
{
    class PrivateJob:  AJob<DefaultJobCommand>
    {
       protected override void Execute(DefaultJobCommand command)
       {
           throw new NotImplementedException();
       }
    }


    public abstract class AJob: AJob<DefaultJobCommand>
    {
        protected override void Execute(DefaultJobCommand command = null)
        {
            Execute();
        }

        protected abstract void Execute();
    }

    public abstract class AJob<T> where T:IJobCommand
    {
        protected abstract void Execute(T command=default(T));

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