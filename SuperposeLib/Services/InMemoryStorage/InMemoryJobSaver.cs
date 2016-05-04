using SuperposeLib.Interfaces.Storage;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using SuperposeLib.Extensions;
using SuperposeLib.Models;

namespace SuperposeLib.Services.InMemoryStorage
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