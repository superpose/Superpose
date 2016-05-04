using System;
using System.Linq;
using SuperposeLib.Core;
using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;
using SuperposeLib.Services.DefaultConverter;
using SuperposeLib.Services.InMemoryStorage;
using SuperposeLib.Tests.Jobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperposeLib.Extensions;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Tests
{
    [TestClass]
    public class job_tests_scheduling
    {
        [TestMethod]
        public void it_can_schedule_job()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.ScheduleJob(typeof(TestJob), DateTime.UtcNow.AddSeconds(3));
                System.Threading.Thread.Sleep(3000);
                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobType, typeof(TestJob));
                Assert.AreEqual(result.JobId, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.First(), JobExecutionStatus.Passed);
                Assert.AreEqual(result.JobStateType, JobStateType.Successfull);
            }

        }

        [TestMethod]
        public void it_can_schedule_job2()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.ScheduleJob(typeof(TestJob), DateTime.UtcNow.AddSeconds(3));
            
                var result = factory.ProcessJob(jobId);
                Assert.IsNull(result);
                result = factory.GetJobLoad(jobId);
                Assert.AreEqual(result.JobType, typeof(TestJob));
                Assert.AreEqual(result.JobId, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 0);
                Assert.AreEqual(result.JobStateType, JobStateType.Queued);
            }

        }

        [TestMethod]
        public void it_can_schedule_job_virtual_time_future()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter, new VirtualTime(DateTime.UtcNow.AddSeconds(4)));
                var jobId = factory.ScheduleJob(typeof(TestJob), DateTime.UtcNow.AddSeconds(3));

                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobType, typeof(TestJob));
                Assert.AreEqual(result.JobId, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.First(), JobExecutionStatus.Passed);
                Assert.AreEqual(result.JobStateType, JobStateType.Successfull);
            }
        }

        [TestMethod]
        public void it_can_schedule_job_virtual_time_future2()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter, new VirtualTime(DateTime.UtcNow.AddYears(4)));
                var jobId = factory.ScheduleJob(typeof(TestJob), DateTime.UtcNow.AddYears(3));

                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobType, typeof(TestJob));
                Assert.AreEqual(result.JobId, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.First(), JobExecutionStatus.Passed);
                Assert.AreEqual(result.JobStateType, JobStateType.Successfull);
            }
        }
    }
}