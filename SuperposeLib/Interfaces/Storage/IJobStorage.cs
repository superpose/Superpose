using System;

namespace SuperposeLib.Interfaces.Storage
{
    public interface IJobStorage : IDisposable
    {
        IJobSaver JobSaver { set; get; }
        IJobLoader JobLoader { set; get; }
        IJobStorageReseter JobStorageReseter { set; get; }
    }
}