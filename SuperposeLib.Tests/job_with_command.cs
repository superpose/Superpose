using System;
using System.Linq;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using Superpose.Owin.Storage.InMemory;
using Superpose.StorageInterface;
using SuperposeLib.Core;
using SuperposeLib.Extensions;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Owin;
using SuperposeLib.Tests.Jobs;

namespace SuperposeLib.Tests
{
    [TestClass]
    public class job_with_command : TestHarness
    {

        public class TestStartup
        {
            public void Configuration(IAppBuilder app)
            {
                app.UseSuperposeInMemoryStorage();
                app.UseSuperposeServer();
            }
        }



        [TestMethod]
        public void test_using_owin_mulitple_continuation_job()
        {
            const string baseAddress = "http://*:8118/";
            using (WebApp.Start<TestStartup>(new StartOptions(baseAddress)))
            {
                Console.WriteLine("Server started");
                var command1 = new TestCommand() { MyName = "tester1" };
                var command2 = new TestCommand() { MyName = "tester2" };
                var command3 = new TestCommand() { MyName = "tester3" };
                var command4 = new TestCommand() { MyName = "tester4" };


                var jobId = JobHandler.EnqueueJob<JobWithCommand, TestCommand>(command1,
                            (continuation2) => continuation2.EnqueueJob<JobWithCommand, TestCommand>(command2,
                             (continuation3) => continuation3.EnqueueJob<JobWithCommand, TestCommand>(command2,
                              (continuation4) => continuation4.EnqueueJob<JobWithCommand, TestCommand>(command2)
                             )
                            )
                           );

                AssertAwait(() => EnsureJobHasRun(jobId), 5000);

                var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage();
                var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();

                IJobFactory factory = new JobFactory(storage, converter);
                var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                Assert.AreEqual(statistics.TotalNumberOfJobs, 4);
                Assert.AreEqual(statistics.TotalSuccessfullJobs, 4);
                Assert.AreEqual(statistics.TotalFailedJobs, 0);
                Assert.AreEqual(statistics.TotalProcessingJobs, 0);
            }
        }





        [TestMethod]
        public void test_using_owin_continuation_job()
        {
            const string baseAddress = "http://*:8118/";
            using (WebApp.Start<TestStartup>(new StartOptions(baseAddress)))
            {
                Console.WriteLine("Server started");
                var command1 = new TestCommand() {MyName = "tester1"};
                var command2 = new TestCommand() { MyName = "tester2" };

                var jobId = JobHandler.EnqueueJob<JobWithCommand, TestCommand>(command1, 
                            (continuation) => continuation.EnqueueJob<JobWithCommand, TestCommand>(command2));

                AssertAwait(() => EnsureJobHasRun(jobId), 5000);

                var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage();
                var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();

                IJobFactory factory = new JobFactory(storage, converter);
                var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                Assert.AreEqual(statistics.TotalNumberOfJobs, 2);
                Assert.AreEqual(statistics.TotalSuccessfullJobs, 2);
                Assert.AreEqual(statistics.TotalFailedJobs, 0);
                Assert.AreEqual(statistics.TotalProcessingJobs, 0);
            }
        }
        [TestMethod]
        public void test_using_owin()
        {
            const string baseAddress = "http://*:8118/";
            using (WebApp.Start<TestStartup>(new StartOptions(baseAddress)))
            {
                Console.WriteLine("Server started");
                var jobId = JobHandler.EnqueueJob<JobWithCommand, TestCommand>(new TestCommand() { MyName = "tester" });
                AssertAwait(() => EnsureJobHasRun(jobId), 5000);
                var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage();
                var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();

                IJobFactory factory = new JobFactory(storage, converter);
                var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                Assert.AreEqual(statistics.TotalNumberOfJobs, 1);
                Assert.AreEqual(statistics.TotalSuccessfullJobs, 1);
                Assert.AreEqual(statistics.TotalFailedJobs, 0);
                Assert.AreEqual(statistics.TotalProcessingJobs, 0);
            }
        }

        private static void EnsureJobHasRun(string jobId)
        {
            var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage();
            var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();

            IJobFactory factory = new JobFactory(storage, converter);

            var existingResult = factory.GetJobLoad(jobId);

            Assert.AreEqual(existingResult.JobTypeFullName, typeof (JobWithCommand).AssemblyQualifiedName);
            Assert.AreEqual(existingResult.Id, jobId);
            Assert.IsNotNull(existingResult);
            Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 1);
            Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Passed);
            Assert.AreEqual(existingResult.JobStateTypeName, JobStateType.Successfull.GetJobStateTypeName());

          
        }


        [TestMethod]
        public void test_using_owin1()
        {
            const string baseAddress = "http://*:8118/";
            using (WebApp.Start<TestStartup>(new StartOptions(baseAddress)))
            {
                Console.WriteLine("Server started");
                var jobId= JobHandler.EnqueueJob<JobWithCommand,TestCommand>(new TestCommand() { MyName = "tester" });
                var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage();
                var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();
                var runner = new JobRunner(storage, converter);
                IJobFactory factory = new JobFactory(storage, converter);

                var result = runner.Run(null, null);

                Assert.IsTrue(result);
                var existingResult = factory.GetJobLoad(jobId);

                Assert.AreEqual(existingResult.JobTypeFullName, typeof(JobWithCommand).AssemblyQualifiedName);
                Assert.AreEqual(existingResult.Id, jobId);
                Assert.IsNotNull(existingResult);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 1);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Passed);
                Assert.AreEqual(existingResult.JobStateTypeName, JobStateType.Successfull.GetJobStateTypeName());

                var statistics = factory.JobStorage.JobLoader.GetJobStatistics();
                Assert.AreEqual(statistics.TotalNumberOfJobs, 1);
                Assert.AreEqual(statistics.TotalSuccessfullJobs, 1);
                Assert.AreEqual(statistics.TotalFailedJobs, 0);
                Assert.AreEqual(statistics.TotalProcessingJobs, 0);


            }

            
        }




        [TestMethod]
        public void test1()
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
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Count(x => x == JobExecutionStatus.Passed), 1);
                Assert.AreEqual(existingResult.PreviousJobExecutionStatusList.Last(), JobExecutionStatus.Passed);
                Assert.AreEqual(existingResult.JobStateTypeName,JobStateType.Successfull.GetJobStateTypeName());

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

    public class TestCommand:AJobCommand
    {
        public string MyName { set; get; }
    }
}