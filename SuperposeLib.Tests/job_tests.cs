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
    public class job_tests_queueing
    {
        [TestMethod]
        public void it_can_serialize_and_deserialize_job()
        {
            IJobConverter converter = new DefaultJobConverter(new DefaultJobParser(), new DefaultJobSerializer());
            var serializedData = converter.Serialize(new JobLoad { JobType = typeof(TestJob) });
            var load = converter.Parse(serializedData);
            Assert.AreEqual(load.JobType.Name, typeof(TestJob).Name);
        }

        [TestMethod]
        public void it_can_queue_job()
        {
            IJobFactory factory = new JobFactory(
                new InMemoryJobStorage(new InMemoryJobSaver(), new InMemoryJobLoader()),
                new DefaultJobConverter(new DefaultJobParser(), new DefaultJobSerializer()));
            var jobId = factory.QueueJob(typeof(TestJob));
            Assert.IsFalse(string.IsNullOrEmpty(jobId));
        }

        [TestMethod]
        public void it_can_persist_job()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJob));
                var jobLoad = factory.GetJobLoad(jobId);
                Assert.IsNotNull(jobLoad);
                factory.InstantiateJobComponent(jobLoad);
                Assert.IsNotNull(jobLoad.Job);
                Assert.AreEqual(jobLoad.Job.GetType().Name, typeof(TestJob).Name);
            }
        }

        [TestMethod]
        public void it_can_persist_job_with_differnt_job_factories()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJob));

                IJobFactory factoryNew = new JobFactory(storage, converter);
                var jobLoad = (JobLoad)factoryNew.GetJobLoad(jobId);

                Assert.IsNotNull(jobLoad);
                factory.InstantiateJobComponent(jobLoad);
                Assert.IsNotNull(jobLoad.Job);
                Assert.AreEqual(jobLoad.Job.GetType().Name, typeof(TestJob).Name);
            }
        }

        [TestMethod]
        public void it_can_run_job()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJob));
                var jobLoad = (JobLoad)factory.GetJobLoad(jobId);
                factory.InstantiateJobComponent(jobLoad);
                var result = jobLoad.Job.RunJob();
                Assert.IsTrue(result.IsSuccessfull);
            }
        }

        [TestMethod]
        public void it_can_que_job()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJob));
                var jobLoad = factory.GetJobLoad(jobId);

                Assert.AreEqual(jobLoad.JobType, typeof(TestJob));
                Assert.AreEqual(jobLoad.JobId, jobId);
                Assert.IsNotNull(jobLoad);
                Assert.AreEqual(jobLoad.HistoricFailureCount(), 0);
                Assert.AreEqual(jobLoad.PreviousJobExecutionStatusList.Count, 0);
                Assert.AreEqual(jobLoad.PreviousJobExecutionStatusList.FirstOrDefault(), JobExecutionStatus.Unknown);
                Assert.AreEqual(jobLoad.JobStateType, JobStateType.Queued);
            }
        }

        [TestMethod]
        public void it_can_process_a_queued_job()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJob));

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
        public void it_can_process_a_queued_job_first_failure()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJobThatPassesAfter2Tryals));

                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobType, typeof(TestJobThatPassesAfter2Tryals));
                Assert.AreEqual(result.JobId, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 1);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.First(), JobExecutionStatus.Failed);
                Assert.AreEqual(result.JobStateType, JobStateType.Queued);
            }
        }

        [TestMethod]
        public void it_can_process_a_queued_job_second_failure()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJobThatPassesAfter2Tryals));

                factory.ProcessJob(jobId);
                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobType, typeof(TestJobThatPassesAfter2Tryals));
                Assert.AreEqual(result.JobId, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 2);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 2);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Failed), 2);
                Assert.AreEqual(result.JobStateType, JobStateType.Queued);
            }
        }


        [TestMethod]
        public void it_can_process_a_queued_job_third_time()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJobThatPassesAfter2Tryals));

                factory.ProcessJob(jobId);
                factory.ProcessJob(jobId);
                var result = factory.ProcessJob(jobId);
                Assert.IsNotNull(result);

                Assert.AreEqual(result.JobType, typeof(TestJobThatPassesAfter2Tryals));
                Assert.AreEqual(result.JobId, jobId);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.HistoricFailureCount(), 3);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count, 3);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Failed), 3);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 0);
                Assert.AreEqual(result.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Failed);
                Assert.AreEqual(result.JobStateType, JobStateType.Successfull);
            }
        }


        [TestMethod]
        public void process_a_queued_job_forth_time()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJobThatPassesAfter2Tryals));

                factory.ProcessJob(jobId);
                factory.ProcessJob(jobId);
                factory.ProcessJob(jobId);
                var result = factory.ProcessJob(jobId);
                Assert.IsNull(result, "job should not be processed again once succeeded");

                var existingResult = factory.GetJobLoad(jobId);

                Assert.AreEqual(existingResult.JobType, typeof(TestJobThatPassesAfter2Tryals));
                Assert.AreEqual(existingResult.JobId, jobId);
                Assert.IsNotNull(existingResult);
                Assert.AreEqual(existingResult.HistoricFailureCount(), 3);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count, 3);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Failed), 3);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 0);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Failed);
                Assert.AreEqual(existingResult.JobStateType, JobStateType.Successfull);
            }
        }


        [TestMethod]
        public void process_a_queued_job_generic_time()
        {
            IJobStoragefactory storageFactory = new InMemoryJobStoragefactory();
            IJobConverterFactory converterFactory = new DefaultJobConverterFactory();
            var converter = converterFactory.CretateConverter();
            using (var storage = storageFactory.CreateJobStorage())
            {
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(TestJobThatPassesAfter2Tryals));
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
                        Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Failed), i);
                    }


                    Assert.AreEqual(existingResult.JobType, typeof(TestJobThatPassesAfter2Tryals));
                    Assert.AreEqual(existingResult.JobId, jobId);
                    Assert.IsNotNull(existingResult);

                    Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 0);
                    Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Failed);
                    Assert.AreEqual(existingResult.JobStateType,
                        i >= 3 ? JobStateType.Successfull : JobStateType.Queued);
                }
            }
        }
    }
}