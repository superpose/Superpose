using SuperposeLib.Interfaces.Storage;

namespace SuperposeLib.Services.InMemoryStorage
{
    public class InMemoryJobStoragefactory : IJobStoragefactory
    {
        public IJobStorage CreateJobStorage()
        {
            return new InMemoryJobStorage(new InMemoryJobSaver(), new InMemoryJobLoader());
        }
    }
}