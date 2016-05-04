using SuperposeLib.Interfaces.Storage;

namespace SuperposeLib.Services.InMemoryStorage
{
    public class InMemoryJobStoragefactory : IJobStoragefactory
    {
        private IJobStorage JobStorage { set; get; }

        public IJobStorage CreateJobStorage()
        {
            return JobStorage ?? (JobStorage = new InMemoryJobStorage(new InMemoryJobSaver(), new InMemoryJobLoader()));
        }
    }
}