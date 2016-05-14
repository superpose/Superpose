using System.Collections.Concurrent;
using Superpose.StorageInterface;

namespace Superpose.Storage.InMemory
{
    public class InMemoryJobStorageMemoryStore
    {
        public static ConcurrentDictionary<string, ConcurrentDictionary<string, SerializableJobLoad>> MemoryStore { set; get  ;}

        public static void InitializeStoreWithInstance(string instance)
        {
            InMemoryJobStorageMemoryStore.MemoryStore = InMemoryJobStorageMemoryStore.MemoryStore ??
                                                       new ConcurrentDictionary
                                                           <string, ConcurrentDictionary<string, SerializableJobLoad>>();
            if (!InMemoryJobStorageMemoryStore.MemoryStore.ContainsKey(instance))
            {
                InMemoryJobStorageMemoryStore.MemoryStore.GetOrAdd(instance, new ConcurrentDictionary<string, SerializableJobLoad>());
            }
        }
    }
}