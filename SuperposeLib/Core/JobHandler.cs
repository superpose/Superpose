using Superpose.StorageInterface;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public class JobHandler
    {
        public static string EnqueueJob<T>() where T : AJob
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage())
            {
                var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();
                IJobFactory factory = new JobFactory(storage, converter);
                var jobId = factory.QueueJob(typeof(T));
                return jobId;
            }
        }
    }
}