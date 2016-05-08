using System;
using Superpose.StorageInterface;
using SuperposeLib.Core;

namespace SuperposeLib.Tests.Jobs
{
    public class TestJobThatPassesAfter2Tryals : AJob
    {
        public override SuperVisionDecision Supervision(Exception reaon, int totalNumberOfHistoricFailures)
        {
            return totalNumberOfHistoricFailures > 2 ? SuperVisionDecision.Pass : SuperVisionDecision.ReQueue;
        }

        protected override void Execute()
        {
            throw new Exception();
        }
    }
}