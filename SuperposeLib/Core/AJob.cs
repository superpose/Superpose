using SuperposeLib.Models;
using System;

namespace SuperposeLib.Core
{
    public abstract class AJob
    {
        protected abstract void Execute();
        
        public virtual SuperVisionDecision Supervision(Exception reaon, int totalNumberOfHistoricFailures)
        {
            return totalNumberOfHistoricFailures > 10 ? SuperVisionDecision.Fail : SuperVisionDecision.ReQueue;
        }

        public JobResult RunJob()
        {
            var result = new JobResult();
            try
            {
                Execute();
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