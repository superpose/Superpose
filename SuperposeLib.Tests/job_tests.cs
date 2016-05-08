using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpose.Storage.InMemory;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Core;
using SuperposeLib.Extensions;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Models;
using SuperposeLib.Services.DefaultConverter;
using SuperposeLib.Services.InMemoryStorage;
using SuperposeLib.Tests.Jobs;

namespace SuperposeLib.Tests
{
    [TestClass]
    public class job_tests_queueing : TestHarness
    {
        [TestMethod]
        public void it_can_serialize_and_deserialize_job()
        {
            IJobConverter converter = new DefaultJobConverter(new DefaultJobParser(), new DefaultJobSerializer());
            var serializedData =
                converter.SerializeJobLoad(new JobLoad {JobTypeFullName = typeof (TestJob).AssemblyQualifiedName});
            var load = converter.Parse(serializedData);
            Assert.AreEqual(load.JobTypeFullName, typeof (TestJob).AssemblyQualifiedName);
        }

        [TestMethod]
        public void it_can_queue_job()
        {
            IJobFactory factory = new JobFactory(
                new InMemoryJobStorage(new InMemoryJobSaver(), new InMemoryJobLoader(), new InMemoryJobResetter()),
                new DefaultJobConverter(new DefaultJobParser(), new DefaultJobSerializer()));
            var jobId = factory.QueueJob(typeof (TestJob));
            Assert.IsFalse(string.IsNullOrEmpty(jobId));
        }

        [TestMethod]
        public void it_can_persist_job()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJob));
                var jobLoad = factory.GetJobLoad(jobId);
                Assert.IsNotNull(jobLoad);
                factory.InstantiateJobComponent(jobLoad);
                Assert.IsNotNull(jobLoad.Job);
                Assert.AreEqual(jobLoad.Job.GetType().Name, typeof (TestJob).Name);
            }
        }

        [TestMethod]
        public void it_can_persist_job_with_differnt_job_factories()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJob));

                IJobFactory factoryNew = new JobFactory(storage, converter);
                var jobLoad = factoryNew.GetJobLoad(jobId);

                Assert.IsNotNull(jobLoad);
                factory.InstantiateJobComponent(jobLoad);
                Assert.IsNotNull(jobLoad.Job);
                Assert.AreEqual(jobLoad.Job.GetType().Name, typeof (TestJob).Name);
            }
        }

        [TestMethod]
        public void it_can_run_job()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJob));
                var jobLoad = factory.GetJobLoad(jobId);
                factory.InstantiateJobComponent(jobLoad);
                var result = jobLoad.Job.RunJob(null);
                Assert.IsTrue(result.IsSuccessfull);
            }
        }

        [TestMethod]
        public void it_can_que_job()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJob));
                var jobLoad = factory.GetJobLoad(jobId);

                Assert.AreEqual(jobLoad.JobTypeFullName, typeof (TestJob).AssemblyQualifiedName);
                Assert.AreEqual(jobLoad.Id, jobId);
                Assert.IsNotNull(jobLoad);
                Assert.AreEqual(jobLoad.HistoricFailureCount(), 0);
                Assert.AreEqual(jobLoad.PreviousJobExecutionStatusList.Count, 0);
                Assert.AreEqual(jobLoad.PreviousJobExecutionStatusList.FirstOrDefault(), JobExecutionStatus.Unknown);
                Assert.AreEqual(jobLoad.JobStateTypeName, Enum.GetName(typeof (JobStateType), JobStateType.Queued));
            }
        }

        [TestMethod]
        public void it_can_process_a_queued_job()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJob));

                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobTypeFullName, typeof (TestJob).AssemblyQualifiedName);
                Assert.AreEqual(result.Id, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.First(), JobExecutionStatus.Passed);
                Assert.AreEqual(result.JobStateTypeName, Enum.GetName(typeof (JobStateType), JobStateType.Successfull));
            }
        }

        [TestMethod]
        public void it_can_process_a_queued_job_first_failure()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));

                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobTypeFullName, typeof (TestJobThatPassesAfter2Tryals).AssemblyQualifiedName);
                Assert.AreEqual(result.Id, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.First(), JobExecutionStatus.Failed);
                Assert.AreEqual(result.JobStateTypeName, Enum.GetName(typeof (JobStateType), JobStateType.Queued));
            }
        }

        [TestMethod]
        public void it_can_process_a_queued_job_second_failure()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));

                factory.ProcessJob(jobId);
                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobTypeFullName, typeof (TestJobThatPassesAfter2Tryals).AssemblyQualifiedName);
                Assert.AreEqual(result.Id, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 2);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 2);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Failed), 2);
                Assert.AreEqual(result.JobStateTypeName, Enum.GetName(typeof (JobStateType), JobStateType.Queued));
            }
        }


        [TestMethod]
        public void it_can_process_a_queued_job_third_time()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));

                factory.ProcessJob(jobId);
                factory.ProcessJob(jobId);
                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobTypeFullName, typeof (TestJobThatPassesAfter2Tryals).AssemblyQualifiedName);
                Assert.AreEqual(result.Id, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 3);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 3);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Failed), 3);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Failed);
                Assert.AreEqual(result.JobStateTypeName, Enum.GetName(typeof (JobStateType), JobStateType.Successfull));
            }
        }


        [TestMethod]
        public void process_a_queued_job_forth_time()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof (TestJobThatPassesAfter2Tryals));

                factory.ProcessJob(jobId);
                factory.ProcessJob(jobId);
                factory.ProcessJob(jobId);
                var result = factory.ProcessJob(jobId);
                Assert.IsNull(result, "job should not be processed again once succeeded");

                var existingResult = factory.GetJobLoad(jobId);

                Assert.AreEqual(existingResult.JobTypeFullName,
                    typeof (TestJobThatPassesAfter2Tryals).AssemblyQualifiedName);
                Assert.AreEqual(existingResult.Id, jobId);
                Assert.IsNotNull(existingResult);
                Assert.AreEqual(existingResult.HistoricFailureCount(), 3);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count, 3);
                Assert.AreEqual(
                    existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Failed), 3);
                Assert.AreEqual(
                    existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 0);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Failed);
                Assert.AreEqual(existingResult.JobStateTypeName,
                    Enum.GetName(typeof (JobStateType), JobStateType.Successfull));
            }
        }


        [TestMethod]
        public void process_a_queued_job_generic_time()
        {
            //IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            //IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = ConverterFactory.CretateConverter();
            using (var storage = StorageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
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
                            existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Failed), i);
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
            }
        }
    }
}