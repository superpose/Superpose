using System;

namespace Superpose.StorageInterface
{
    public interface IJobStorage : IDisposable
    {
        IJobSaver JobSaver { set; get; }
        IJobLoader JobLoader { set; get; }
        IJobStorageReseter JobStorageReseter { set; get; }
    }
}