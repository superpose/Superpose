using System.Collections.Concurrent;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;
using SuperposeLib.Services.InMemoryStorage;

namespace Superpose.Storage.InMemory
{
    public class InMemoryJobResetter : IJobStorageReseter
    {
        public void ReSet()
        {
            InMemoryJobStorageMemoryStore.MemoryStore=new ConcurrentDictionary<string, JobLoad>();
        }
    }
}