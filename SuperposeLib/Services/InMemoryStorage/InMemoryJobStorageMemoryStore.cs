using System.Collections.Concurrent;
using SuperposeLib.Models;

namespace SuperposeLib.Services.InMemoryStorage
{
    public class InMemoryJobStorageMemoryStore
    {
        public static ConcurrentDictionary<string, JobLoad> MemoryStore = new ConcurrentDictionary<string, JobLoad>();
    }
}