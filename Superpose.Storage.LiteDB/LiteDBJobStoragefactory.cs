using System;
using Superpose.StorageInterface;

namespace Superpose.Storage.LiteDB
{
    public class LiteDbJobStoragefactory : IJobStoragefactory
    {
        public string InstanceId { set; get; }

        private IJobStorage JobStorage { set; get; }

        public string GetCurrentExecutionInstance()
        {
            return InstanceId;
        }

        public IJobStorage GetJobStorage(string instanceId = null)
        {
            InstanceId = instanceId ?? Guid.Empty.ToString();
            return JobStorage ??
                   (JobStorage =
                       new LiteDBJobStorage(new LiteDBJobSaver(), new LiteDbJobLoader(), new LiteDBStorageResetter()));
        }
    }
}