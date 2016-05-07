using System;

namespace Superpose.StorageInterface
{
    public interface IJobSaver : IDisposable
    {
        void SaveNew(string data, string Id);

        void Update(string data, string Id);
    }
}