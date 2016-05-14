using Newtonsoft.Json;
using Superpose.StorageInterface;
using SuperposeLib.Extensions;

namespace Superpose.Storage.InMemory
{
    public class InMemoryJobSaver : IJobSaver
    {
        protected string Instance { private set; get; }

        public InMemoryJobSaver(string instance)
        {
            Instance = instance;
            InMemoryJobStorageMemoryStore.InitializeStoreWithInstance(Instance);
        }

        public void SaveNew(string data, string Id)
        {
            InMemoryJobStorageMemoryStore.MemoryStore[Instance].GetOrAdd(Id,
                JsonConvert.DeserializeObject<SerializableJobLoad>(data));
        }

        public void Update(string data, string Id)
        {
            InMemoryJobStorageMemoryStore.MemoryStore[Instance].AddOrUpdate(Id,
                JsonConvert.DeserializeObject<SerializableJobLoad>(data));
        }

        public void Dispose()
        {
            //  InMemoryJobStorageMemoryStore.MemoryStore = new ConcurrentDictionary<string, SerializableJobLoad>();
        }
    }
}