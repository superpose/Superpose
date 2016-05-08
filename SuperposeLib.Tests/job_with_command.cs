using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpose.StorageInterface;
using SuperposeLib.Core;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Tests.Jobs;

namespace SuperposeLib.Tests
{
    [TestClass]
    public class job_with_command : TestHarness
    {
        [TestMethod]
        public void test()
        {
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob<JobWithCommand>(new TestCommand() {MyName = "tester"});

                var runner = new JobRunner(storage, converter);
                var result = runner.Run(null, null);

                Assert.IsTrue(result);
                var existingResult = factory.GetJobLoad(jobId);

                Assert.AreEqual(existingResult.JobTypeFullName,typeof (JobWithCommand).AssemblyQualifiedName);
                Assert.AreEqual(existingResult.Id, jobId);
                Assert.IsNotNull(existingResult);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 0);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Failed);
                Assert.AreEqual(existingResult.JobStateTypeName,Enum.GetName(typeof (JobStateType), JobStateType.Successfull));

                var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                Assert.AreEqual(statistics.TotalNumberOfJobs, 1);
                Assert.AreEqual(statistics.TotalSuccessfullJobs, 1);
                Assert.AreEqual(statistics.TotalFailedJobs, 0);
                Assert.AreEqual(statistics.TotalProcessingJobs, 0);
            }
        }

    }

    public class JobWithCommand:AJob<TestCommand>
    {
        protected override void Execute(TestCommand command)
        {
            Console.WriteLine(command.MyName);
        }
    }

    public class TestCommand
    {
        public string MyName { set; get; }
    }
}