using Superpose.StorageInterface;

namespace Superpose.Storage.SqlServer
{
    public class SqlServerStoragefactory : IJobStoragefactory
    {
        private IJobStorage JobStorage { set; get; }

        public IJobStorage CreateJobStorage()
        {
            return JobStorage ??
                   (JobStorage =
                       new SqlServerStorage(new SqlServerJobSaver(), new SqlServerJobLoader(),
                           new SqlServerJobResetter()));
        }
    }
}