using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperposeLib.Core;
using SuperposeLib.Extensions;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Models;
using SuperposeLib.Tests.Jobs;

namespace SuperposeLib.Tests
{
    [TestClass]
    public class job_tests_multiple_jobs_queuing : TestHarness
    {
        [TestMethod]
        public void process_a_queued_job_generic_time()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                const int noOfJobs = 10;
                for (var j = 1; j <= noOfJobs; j++)
                {
                    var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));
                    const int noOfCircles = 10;
                    for (var i = 1; i <= noOfCircles; i++)
                    {
                        var result = factory.ProcessJob(jobId);
                        var existingResult = factory.GetJobLoad(jobId);
                        if (i > 3)
                        {
                            Assert.IsNull(result);
                        }
                        else
                        {
                            Assert.IsNotNull(result);
                            Assert.AreEqual(existingResult.HistoricFailureCount(), i);
                            Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count, i);
                            Assert.AreEqual(
                                existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Failed),
                                i);
                        }
                        Assert.AreEqual(existingResult.JobTypeFullName,
                            typeof (TestJobThatPassesAfter2Tryals).AssemblyQualifiedName);
                        Assert.AreEqual(existingResult.Id, jobId);
                        Assert.IsNotNull(existingResult);
                        Assert.AreEqual(
                            existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 0);
                        Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Failed);
                        Assert.AreEqual(existingResult.JobStateTypeName,
                            i >= 3
                                ? Enum.GetName(typeof (JobStateType), JobStateType.Successfull)
                                : Enum.GetName(typeof (JobStateType), JobStateType.Queued));
                    }

                    var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                    Assert.AreEqual(statistics.TotalNumberOfJobs, j);
                    Assert.AreEqual(statistics.TotalSuccessfullJobs, j);
                    Assert.AreEqual(statistics.TotalFailedJobs, 0);
                    Assert.AreEqual(statistics.TotalProcessingJobs, 0);
                }
            }
        }
    }
}