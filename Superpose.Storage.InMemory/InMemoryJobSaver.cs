using Newtonsoft.Json;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Storage;
using SuperposeLib.Extensions;
using SuperposeLib.Services.InMemoryStorage;

namespace Superpose.Storage.InMemory
{
    public class InMemoryJobSaver : IJobSaver
    {
        public void SaveNew(string data, string Id)
        {
            InMemoryJobStorageMemoryStore.MemoryStore.GetOrAdd(Id,
                JsonConvert.DeserializeObject<SerializableJobLoad>(data));
        }

        public void Update(string data, string Id)
        {
            InMemoryJobStorageMemoryStore.MemoryStore.AddOrUpdate(Id,
                JsonConvert.DeserializeObject<SerializableJobLoad>(data));
        }

        public void Dispose()
        {
            //  InMemoryJobStorageMemoryStore.MemoryStore = new ConcurrentDictionary<string, SerializableJobLoad>();
        }
    }
}