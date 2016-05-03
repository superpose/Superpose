using System;

namespace SuperposeLib.Interfaces.Storage
{
    public interface IJobSaver : IDisposable
    {
        void SaveNew(string data, string Id);

        void Update(string data, string Id);
    }
}