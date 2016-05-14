using System.Collections.Concurrent;
using Superpose.StorageInterface;

namespace Superpose.Storage.InMemory
{
    public class InMemoryJobResetter : IJobStorageReseter
    {
        protected string Instance { private set; get; }

        public InMemoryJobResetter(string instance)
        {
            Instance = instance;
            InMemoryJobStorageMemoryStore.InitializeStoreWithInstance(Instance);
        }
        public void ReSet()
        {
            InMemoryJobStorageMemoryStore.MemoryStore[Instance] = new ConcurrentDictionary<string, SerializableJobLoad>();
        }
    }
}