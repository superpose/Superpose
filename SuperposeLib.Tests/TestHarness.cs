using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpose.Storage.InMemory;
using Superpose.Storage.SqlServer;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Services.DefaultConverter;

namespace SuperposeLib.Tests
{
    public class TestHarness
    {
        protected IJobStoragefactory StorageFactory { set; get; }
        protected IJobConverterFactory ConverterFactory { set; get; }

        [TestInitialize]
        public void SetUpMethod()
        {
            //StorageFactory = new SqlServerStoragefactory();
            StorageFactory = new InMemoryJobStoragefactory();
            // StorageFactory = new LiteDBJobStoragefactory();
            ConverterFactory = new DefaultJobConverterFactory();
        }

        [TestCleanup]
        public void TeardownMethod()
        {
            StorageFactory.CreateJobStorage().JobStorageReseter.ReSet();
        }

        public void AssertAwait(Action action, int durationMilliseconds, int sleepIntervalMilliseconds = 50)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            var now = DateTime.Now;
            var passed = false;
            var lastException = new Exception();

            while ((DateTime.Now - now).TotalMilliseconds <= durationMilliseconds)
            {
                try
                {
                    action();
                    passed = true;
                    break;
                }
                catch (Exception e)
                {
                    lastException = e;
                }
                Thread.Sleep(sleepIntervalMilliseconds);
            }
            if (!passed)
            {
                throw new Exception("Could not pass in " + durationMilliseconds + " ms ", lastException);
            }
        }
    }
}