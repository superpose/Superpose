using System;

namespace SuperposeLib.Core.Jobs
{
    public class CoreJobThatFails : AJob
    {
        public CoreJobThatFails(Exception exception, string jobTypeFullName)
        {
            Exception = exception;
            JobTypeFullName = jobTypeFullName;
        }

        private Exception Exception { get; }
        private string JobTypeFullName { get; }

        public override SuperVisionDecision Supervision(Exception reaon, int totalNumberOfHistoricFailures)
        {
            return SuperVisionDecision.Fail;
        }

        protected override void Execute(object command)
        {
            throw new Exception("Unable to run job " + JobTypeFullName, Exception);
        }
    }
}