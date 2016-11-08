using System;
using System.Runtime.Caching;

namespace MySite.Core.Utils
{
    public static class CacheUtil
    {
        public static T Get<T>(string key, Func<T> receiver, int expMin = 10)
        {
            var result = MemoryCache.Default.Get(key);
            if (result == null)
            {
                MemoryCache.Default.Add(key, (result = receiver()), DateTimeOffset.Now.AddMinutes(expMin));
            }

            return (T)result;
        }
    }
}