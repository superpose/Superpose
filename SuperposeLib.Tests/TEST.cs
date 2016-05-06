using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpose.Storage.LiteDB;
using SuperposeLib.Core;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Models;
using SuperposeLib.Tests.Jobs;

namespace SuperposeLib.Tests
{
    [TestClass]
    public class TEST : TestHarness
    {
        [TestMethod]
        public void LightDbTest()
        {
            LiteDbCollectionsFactory.UseLiteDatabase((collection) =>
            {
                
            });
        }


        [TestMethod]
        public void TES_METHOD()
        {
          
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJobThatPassesAfter2Tryals));

                var runner = new JobRunner(storage, converter);
                var result = runner.Run();

                Assert.IsTrue(result);
                var existingResult = factory.GetJobLoad(jobId);

                Assert.AreEqual(existingResult.JobTypeFullName, typeof(TestJobThatPassesAfter2Tryals).AssemblyQualifiedName);
                Assert.AreEqual(existingResult.Id, jobId);
                Assert.IsNotNull(existingResult);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 0);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Failed);
                Assert.AreEqual(existingResult.JobStateTypeName, Enum.GetName(typeof(JobStateType), JobStateType.Successfull));

                var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                Assert.AreEqual(statistics.TotalNumberOfJobs, 1);
                Assert.AreEqual(statistics.TotalSuccessfullJobs, 1);
                Assert.AreEqual(statistics.TotalFailedJobs, 0);
                Assert.AreEqual(statistics.TotalProcessingJobs, 0);

            }
        }
    }
}