using System.Collections.Concurrent;

namespace SuperposeLib.Extensions
{
    internal static class ConcurrentDictionaryExtensions
    {
       
        public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
        {
            dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }
    }
}