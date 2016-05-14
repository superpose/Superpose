using System;
using System.Linq;
using System.Threading;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpose.StorageInterface;
using SuperposeLib.Core;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Services.DefaultConverter;
using SuperposeLib.Tests.Jobs;

namespace SuperposeLib.Tests
{
    [TestClass]
    public class job_tests_job_runner : TestHarness
    {
        [TestMethod]
        public void test_owin()
        {
            using (WebApp.Start<Startup>("http://localhost:12345"))
            {
                using (var storage = SuperposeGlobalConfiguration.StorageFactory.GetJobStorage(Guid.NewGuid().ToString()))
                {
                    var converter = new DefaultJobConverterFactory().CretateConverter();
                    IJobFactory factory = new JobFactory(storage, converter);
                    var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));
                    var passed = false;

                    var iteri = 10;
                    while (iteri > 0 && !passed)
                    {
                        iteri--;

                        Thread.Sleep(1000);
                        try
                        {
                            var existingResult = factory.GetJobLoad(jobId);
                            Assert.AreEqual(existingResult.JobStateTypeName,
                                Enum.GetName(typeof (JobStateType), JobStateType.Successfull));
                            var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                            Assert.AreEqual(statistics.TotalSuccessfullJobs, 1);
                            passed = true;
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    Assert.IsTrue(passed);
                }
            }
        }

        [TestMethod]
        public void test_bare_bone()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.GetJobStorage(Guid.NewGuid().ToString()))
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));
                IJobRunner runner = new DefaultJobRunner(storage, converter);
                runner.Run(null, null);
                var existingResult = factory.GetJobLoad(jobId);
                Assert.AreEqual(existingResult.JobStateTypeName,
                    Enum.GetName(typeof (JobStateType), JobStateType.Successfull));
                var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                Assert.AreEqual(statistics.TotalSuccessfullJobs, 1);
            }
        }


        [TestMethod]
        public void process_a_queued_job()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.GetJobStorage(Guid.NewGuid().ToString()))
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));

                var runner = new DefaultJobRunner(storage, converter);
                var result = runner.Run(null, null);

                Assert.IsTrue(result);
                var existingResult = factory.GetJobLoad(jobId);

                Assert.AreEqual(existingResult.JobTypeFullName,
                    typeof (TestJobThatPassesAfter2Tryals).AssemblyQualifiedName);
                Assert.AreEqual(existingResult.Id, jobId);
                Assert.IsNotNull(existingResult);
                Assert.AreEqual(
                    existingResult.PreviousJobExecutionStatusList?.Split(',').ToList().Count(x => x == JobExecutionStatus.Passed.ToStringName()), 0);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList?.Split(',').ToList().Last(), JobExecutionStatus.Failed.ToStringName());
                Assert.AreEqual(existingResult.JobStateTypeName,
                    Enum.GetName(typeof (JobStateType), JobStateType.Successfull));

                var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                Assert.AreEqual(statistics.TotalNumberOfJobs, 1);
                Assert.AreEqual(statistics.TotalSuccessfullJobs, 1);
                Assert.AreEqual(statistics.TotalFailedJobs, 0);
                Assert.AreEqual(statistics.TotalProcessingJobs, 0);
            }
        }

        [TestMethod]
        public void process_a_queued_job2()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.GetJobStorage(Guid.NewGuid().ToString()))
            {
                IJobFactory factory = new JobFactory(storage, converter);
                const int noOfJobs = 10;
                for (var j = 1; j <= noOfJobs; j++)
                {
                    var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));

                    var runner = new DefaultJobRunner(storage, converter);
                    var result = runner.Run(null, null);
                    Assert.IsTrue(result);
                    var existingResult = factory.GetJobLoad(jobId);

                    Assert.AreEqual(existingResult.JobTypeFullName,
                        typeof (TestJobThatPassesAfter2Tryals).AssemblyQualifiedName);
                    Assert.AreEqual(existingResult.Id, jobId);
                    Assert.IsNotNull(existingResult);
                    Assert.AreEqual(
                        existingResult.PreviousJobExecutionStatusList?.Split(',').ToList().Count(x => x == JobExecutionStatus.Passed.ToStringName()), 0);
                    Assert.AreEqual(existingResult.PreviousJobExecutionStatusList?.Split(',').ToList().Last(), JobExecutionStatus.Failed.ToStringName());
                    Assert.AreEqual(existingResult.JobStateTypeName,
                        Enum.GetName(typeof (JobStateType), JobStateType.Successfull));


                    var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                    Assert.AreEqual(statistics.TotalNumberOfJobs, j);
                    Assert.AreEqual(statistics.TotalSuccessfullJobs, j);
                    Assert.AreEqual(statistics.TotalFailedJobs, 0);
                    Assert.AreEqual(statistics.TotalProcessingJobs, 0);
                }
            }
        }

        [TestMethod]
        public void process_a_queued_job_generic_time()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.GetJobStorage(Guid.NewGuid().ToString()))
            {
                IJobFactory factory = new JobFactory(storage, converter);
                const int noOfJobs = 10;
                for (var j = 1; j <= noOfJobs; j++)
                {
                    var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));
                    const int noOfCircles = 10;
                    for (var i = 1; i <= noOfCircles; i++)
                    {
                        var runner = new DefaultJobRunner(storage, converter);
                        var result = runner.Run(null, null);
                        Assert.IsTrue(result);
                        var existingResult = factory.GetJobLoad(jobId);

                        Assert.AreEqual(existingResult.JobTypeFullName,
                            typeof (TestJobThatPassesAfter2Tryals).AssemblyQualifiedName);
                        Assert.AreEqual(existingResult.Id, jobId);
                        Assert.IsNotNull(existingResult);
                        Assert.AreEqual(
                            existingResult.PreviousJobExecutionStatusList?.Split(',').ToList().Count(x => x == JobExecutionStatus.Passed.ToStringName()), 0);
                        Assert.AreEqual(existingResult.PreviousJobExecutionStatusList?.Split(',').ToList().Last(), JobExecutionStatus.Failed.ToStringName());
                        Assert.AreEqual(existingResult.JobStateTypeName,
                            Enum.GetName(typeof (JobStateType), JobStateType.Successfull));
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