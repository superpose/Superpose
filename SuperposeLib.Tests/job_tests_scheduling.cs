using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpose.StorageInterface;
using SuperposeLib.Core;
using SuperposeLib.Extensions;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Tests.Jobs;

namespace SuperposeLib.Tests
{
    [TestClass]
    public class job_tests_scheduling : TestHarness
    {
        [TestMethod]
        public void it_can_schedule_job()
        {
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.GetJobStorage(StorageFactory.GetCurrentExecutionInstance()))
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.ScheduleJob(typeof (TestJob), null, DateTime.UtcNow.AddSeconds(3));
                Thread.Sleep(3000);
                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobTypeFullName, typeof (TestJob).AssemblyQualifiedName);
                Assert.AreEqual(result.Id, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList?.Split(',').ToList().Count, 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList?.Split(',').ToList().First(), JobExecutionStatus.Passed.ToStringName());
                Assert.AreEqual(result.JobStateTypeName, Enum.GetName(typeof (JobStateType), JobStateType.Successfull));
            }
        }

        [TestMethod]
        public void it_can_schedule_job2()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.GetJobStorage(StorageFactory.GetCurrentExecutionInstance()))
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.ScheduleJob(typeof (TestJob), null, DateTime.UtcNow.AddSeconds(3));

                var result = factory.ProcessJob(jobId);
                Assert.IsNull(result);
                result = factory.GetJobLoad(jobId);
                Assert.AreEqual(result.JobTypeFullName, typeof (TestJob).AssemblyQualifiedName);
                Assert.AreEqual(result.Id, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList,"");
                Assert.AreEqual(result.JobStateTypeName, Enum.GetName(typeof (JobStateType), JobStateType.Queued));
            }
        }

        [TestMethod]
        public void it_can_schedule_job_virtual_time_future()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.GetJobStorage(StorageFactory.GetCurrentExecutionInstance()))
            {
                IJobFactory factory = new JobFactory(storage, converter, new VirtualTime(DateTime.UtcNow.AddSeconds(4)));
                var jobId = factory.ScheduleJob(typeof (TestJob), null, DateTime.UtcNow.AddSeconds(3));

                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobTypeFullName, typeof (TestJob).AssemblyQualifiedName);
                Assert.AreEqual(result.Id, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList?.Split(',').ToList().Count, 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList?.Split(',').ToList().First(), JobExecutionStatus.Passed.ToStringName());
                Assert.AreEqual(result.JobStateTypeName, Enum.GetName(typeof (JobStateType), JobStateType.Successfull));
            }
        }

        [TestMethod]
        public void it_can_schedule_job_virtual_time_future2()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.GetJobStorage(StorageFactory.GetCurrentExecutionInstance()))
            {
                IJobFactory factory = new JobFactory(storage, converter, new VirtualTime(DateTime.UtcNow.AddYears(4)));
                var jobId = factory.ScheduleJob(typeof (TestJob), null, DateTime.UtcNow.AddYears(3));

                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobTypeFullName, typeof (TestJob).AssemblyQualifiedName);
                Assert.AreEqual(result.Id, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList?.Split(',').ToList().Count, 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList?.Split(',').ToList().First(), JobExecutionStatus.Passed.ToStringName());
                Assert.AreEqual(result.JobStateTypeName, Enum.GetName(typeof (JobStateType), JobStateType.Successfull));
            }
        }
    }
}