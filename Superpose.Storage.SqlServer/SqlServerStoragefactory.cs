using System;
using Superpose.StorageInterface;

namespace Superpose.Storage.SqlServer
{
    public class SqlServerStoragefactory : IJobStoragefactory
    {
       
        public string InstanceId { private set; get; }
        private IJobStorage JobStorage { set; get; }

        public string GetCurrentExecutionInstance()
        {
            return InstanceId;
        }

        public IJobStorage GetJobStorage(string instanceId = null)
        {
            InstanceId = instanceId ?? Guid.NewGuid().ToString();
            return JobStorage ??
                   (JobStorage =
                       new SqlServerStorage(new SqlServerJobSaver(), new SqlServerJobLoader(),
                           new SqlServerJobResetter()));
        }
    }
}