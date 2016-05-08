using System;
using SuperposeLib.Models;
using SuperposeLib.Services.DefaultConverter;

namespace SuperposeLib.Core
{

    public abstract class AJob: AJob<object>
    {

    }

    public abstract class AJob<T>
    {
        protected abstract void Execute(T command);

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