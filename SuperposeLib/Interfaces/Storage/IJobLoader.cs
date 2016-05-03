using System;

namespace SuperposeLib.Interfaces.Storage
{
    public interface IJobLoader : IDisposable
    {
        string Load(string jobId);
    }
}