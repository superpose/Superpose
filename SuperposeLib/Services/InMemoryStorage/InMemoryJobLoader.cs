using SuperposeLib.Interfaces.Storage;
using System.Collections.Concurrent;

namespace SuperposeLib.Services.InMemoryStorage
{
    public class InMemoryJobLoader : IJobLoader
    {
        public string Load(string jobId)
        {
            string data = null;
            if (InMemoryJobStorageMemoryStore.MemoryStore.ContainsKey(jobId))
            {
                data = InMemoryJobStorageMemoryStore.MemoryStore[jobId];
            }

            return data;
        }

        public void Dispose()
        {
            InMemoryJobStorageMemoryStore.MemoryStore = new ConcurrentDictionary<string, string>();
        }
    }
}