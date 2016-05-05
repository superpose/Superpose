using SuperposeLib.Interfaces.Storage;

namespace Superpose.Storage.LiteDB
{
    public class LiteDBJobStoragefactory : IJobStoragefactory
    {
        private IJobStorage JobStorage { set; get; }

        public IJobStorage CreateJobStorage()
        {
            return JobStorage ?? (JobStorage = new LiteDBJobStorage(new LiteDBJobSaver(), new LiteDBJobLoader(),new LiteDBStorageResetter()));
        }
    }
}