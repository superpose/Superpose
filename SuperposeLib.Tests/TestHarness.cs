using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpose.Storage.InMemory;
using Superpose.Storage.LiteDB;
using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Services.DefaultConverter;
using SuperposeLib.Services.InMemoryStorage;

namespace SuperposeLib.Tests
{

    public class TestHarness
    {
        protected IJobStoragefactory StorageFactory { set; get; }
        protected IJobConverterFactory ConverterFactory { set; get; }
        [TestInitialize]
        public void SetUpMethod()
        {
           // StorageFactory = new InMemoryJobStoragefactory();
            StorageFactory = new LiteDBJobStoragefactory();
            ConverterFactory = new DefaultJobConverterFactory();
        }
        [TestCleanup]
        public void TeardownMethod()
        {
            StorageFactory.CreateJobStorage().JobStorageReseter.ReSet();
        }
    }
}
