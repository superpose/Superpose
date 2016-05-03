using System.Collections.Concurrent;

namespace SuperposeLib.Services.InMemoryStorage
{
    public class InMemoryJobStorageMemoryStore
    {
        public static ConcurrentDictionary<string, string> MemoryStore = new ConcurrentDictionary<string, string>();
    }
}