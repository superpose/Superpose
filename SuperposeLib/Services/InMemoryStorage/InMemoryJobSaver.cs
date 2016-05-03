using SuperposeLib.Interfaces.Storage;
using System.Collections.Concurrent;

namespace SuperposeLib.Services.InMemoryStorage
{
    public class InMemoryJobSaver : IJobSaver
    {
        public void SaveNew(string data, string Id)
        {
            InMemoryJobStorageMemoryStore.MemoryStore.GetOrAdd(Id, data);
        }

        public void Update(string data, string Id)
        {
              InMemoryJobStorageMemoryStore.MemoryStore.AddOrUpdate(Id,  data);

           // InMemoryJobStorageMemoryStore.MemoryStore[Id] = data;
          //  InMemoryJobStorageMemoryStore.MemoryStore.GetOrAdd(Id, data);
        }

        public void Dispose()
        {
            InMemoryJobStorageMemoryStore.MemoryStore = new ConcurrentDictionary<string, string>();
        }
    }
}