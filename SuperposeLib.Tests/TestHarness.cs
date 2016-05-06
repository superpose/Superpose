using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpose.Storage.InMemory;
using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Interfaces.Storage;
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
            StorageFactory = new InMemoryJobStoragefactory();
            // StorageFactory = new LiteDBJobStoragefactory();
            ConverterFactory = new DefaultJobConverterFactory();
        }

        [TestCleanup]
        public void TeardownMethod()
        {
            StorageFactory.CreateJobStorage().JobStorageReseter.ReSet();
        }
    }
}