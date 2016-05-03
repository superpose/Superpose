using System.Collections.Concurrent;

namespace SuperposeLib.Services.InMemoryStorage
{
    internal static class ConcurrentDictionaryExtensions
    {
        // Either Add or overwrite
        public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
        {
            dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }
    }
}