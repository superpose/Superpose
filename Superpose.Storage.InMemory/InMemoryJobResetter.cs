using System.Collections.Concurrent;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Storage;
using SuperposeLib.Services.InMemoryStorage;

namespace Superpose.Storage.InMemory
{
    public class InMemoryJobResetter : IJobStorageReseter
    {
        public void ReSet()
        {
            InMemoryJobStorageMemoryStore.MemoryStore = new ConcurrentDictionary<string, SerializableJobLoad>();
        }
    }
}