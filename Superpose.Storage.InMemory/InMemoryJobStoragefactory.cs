using Superpose.StorageInterface;

namespace Superpose.Storage.InMemory
{
    public class InMemoryJobStoragefactory : IJobStoragefactory
    {
        private IJobStorage JobStorage { set; get; }

        public IJobStorage CreateJobStorage()
        {
            return JobStorage ??
                   (JobStorage =
                       new InMemoryJobStorage(new InMemoryJobSaver(), new InMemoryJobLoader(), new InMemoryJobResetter()));
        }
    }
}