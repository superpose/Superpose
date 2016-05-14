using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Core;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Tests.Jobs;

namespace SuperposeLib.Tests
{
    [TestClass]
    public class TEST : TestHarness
    {
        [TestMethod]
        public void TES_METHOD()
        {
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.GetJobStorage(StorageFactory.GetCurrentExecutionInstance()))
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
                    existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 0);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Failed);
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
        public void enqueue_jobs()
        {
            var jobs = new List<string>();
            var converter = ConverterFactory.CretateConverter();
            const int totalNumberOfJobs = 1000000;
            PlaceJobs(totalNumberOfJobs, converter, jobs);
        }

        [TestMethod]
        public void enqueue__process_jobs()
        {
            var jobs = new List<string>();
            var converter = ConverterFactory.CretateConverter();
            const int totalNumberOfJobs = 1000;
            PlaceJobs(totalNumberOfJobs, converter, jobs);
            ProcessJobs(converter);
        }

        [TestMethod]
        public void enqueue__process_validate()
        {
            var jobs = new List<string>();
            var converter = ConverterFactory.CretateConverter();
            const int totalNumberOfJobs = 1000;
            PlaceJobs(totalNumberOfJobs, converter, jobs);
            ProcessJobs(converter);
            ValidatingJobs(converter, jobs);
            ValidateStaistics(converter, totalNumberOfJobs);
        }

        private void PlaceJobs(int totalNumberOfJobs, IJobConverter converter, List<string> jobs)
        {
            Console.WriteLine("placing jobs ... with " + totalNumberOfJobs + " jobs");
            var start = DateTime.Now;
            for (var i = 0; i < totalNumberOfJobs; i++)
            {
                using (var storage = StorageFactory.GetJobStorage(StorageFactory.GetCurrentExecutionInstance()))
                {
                    IJobFactory factory = new JobFactory(storage, converter);
                    var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));
                    jobs.Add(jobId);
                }
            }
           // Console.WriteLine("Done! " + totalNumberOfJobs + " jobs");
            var end = DateTime.Now;
            Console.WriteLine((end - start).TotalMilliseconds+" ms");
        }

        private void ValidateStaistics(IJobConverter converter, int totalNumberOfJobs)
        {
            Console.WriteLine("checking statisctics");
            Console.WriteLine(DateTime.Now);
            using (var storage = StorageFactory.GetJobStorage(StorageFactory.GetCurrentExecutionInstance()))
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                Assert.AreEqual(statistics.TotalNumberOfJobs, totalNumberOfJobs);
                Assert.AreEqual(statistics.TotalSuccessfullJobs, totalNumberOfJobs);
                Assert.AreEqual(statistics.TotalFailedJobs, 0);
                Assert.AreEqual(statistics.TotalProcessingJobs, 0);
            }
            Console.WriteLine("Done! " + totalNumberOfJobs + " jobs");
            Console.WriteLine(DateTime.Now);
        }

        private void ValidatingJobs(IJobConverter converter, List<string> jobs)
        {
            Console.WriteLine("validating jobs ");
            Console.WriteLine(DateTime.Now);
            using (var storage = StorageFactory.GetJobStorage(StorageFactory.GetCurrentExecutionInstance()))
            {
                IJobFactory factory = new JobFactory(storage, converter);

                foreach (var jobId in jobs)
                {
                    var existingResult = factory.GetJobLoad(jobId);
                    Assert.AreEqual(existingResult.JobTypeFullName,
                        typeof (TestJobThatPassesAfter2Tryals).AssemblyQualifiedName);
                    Assert.AreEqual(existingResult.Id, jobId);
                    Assert.IsNotNull(existingResult);
                    Assert.AreEqual(
                        existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 0);
                    Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Failed);
                    Assert.AreEqual(existingResult.JobStateTypeName,
                        Enum.GetName(typeof (JobStateType), JobStateType.Successfull));
                }
            }
            Console.WriteLine("Done!");
            Console.WriteLine(DateTime.Now);
        }

        private void ProcessJobs(IJobConverter converter)
        {
            Console.WriteLine("processing jobs");
            Console.WriteLine(DateTime.Now);
            using (var storage = StorageFactory.GetJobStorage(StorageFactory.GetCurrentExecutionInstance()))
            {
                var runner = new DefaultJobRunner(storage, converter);
                var result = runner.Run(null, null);
                Assert.IsTrue(result);
            }
            Console.WriteLine("Done!");
            Console.WriteLine(DateTime.Now);
        }
    }
}