using System.Collections.Concurrent;
using Newtonsoft.Json;
using SuperposeLib.Extensions;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;
using SuperposeLib.Services.InMemoryStorage;

namespace Superpose.Storage.InMemory
{
    public class InMemoryJobSaver : IJobSaver
    {
        public void SaveNew(string data, string Id)
        {
            InMemoryJobStorageMemoryStore.MemoryStore.GetOrAdd(Id, JsonConvert.DeserializeObject<JobLoad>(data));
        }

        public void Update(string data, string Id)
        {
           InMemoryJobStorageMemoryStore.MemoryStore.AddOrUpdate(Id, JsonConvert.DeserializeObject<JobLoad>(data));
  }

        public void Dispose()
        {
            InMemoryJobStorageMemoryStore.MemoryStore = new ConcurrentDictionary<string, JobLoad>();
        }
    }
}