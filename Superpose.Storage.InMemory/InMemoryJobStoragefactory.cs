using System;
using Superpose.StorageInterface;

namespace Superpose.Storage.InMemory
{
    public class InMemoryJobStoragefactory : IJobStoragefactory
    {
        public string InstanceId { set; get; }

       

        private IJobStorage JobStorage { set; get; }

        public string GetCurrentExecutionInstance()
        {
            return InstanceId;
        }

        public IJobStorage GetJobStorage(string instanceId=null)
        {
            InstanceId = instanceId ?? Guid.Empty.ToString();
            return JobStorage ??
                   (JobStorage =
                       new InMemoryJobStorage(new InMemoryJobSaver(InstanceId), new InMemoryJobLoader(InstanceId), new InMemoryJobResetter(InstanceId)));
        }
    }
}