using Superpose.StorageInterface.Converters;

namespace Superpose.StorageInterface
{
    public class SuperposeGlobalConfiguration
    {
        public static JobQueue JobQueue = new DefaultJobQueue();
        public static IJobConverterFactory JobConverterFactory { set; get; }

        public static IJobStoragefactory StorageFactory { set; get; }
    }
}