using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace GasyTek.ProductService.Core
{
    public static class CacheExtensions
    {
        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var bytes = cache.Get(key);
            if (bytes == null || bytes.Length == 0)
            {
                return default!;
            }

            try
            {
                var strValue = Encoding.UTF8.GetString(bytes);
                return JsonConvert.DeserializeObject<T>(strValue)!;
            }
            catch
            {
                return default!;
            }
        }

        public static void Set<T>(
            this IDistributedCache cache,
            string key, 
            T value,
            DistributedCacheEntryOptions? options = default)
        {
            if (options is null)
            {
                options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
                    SlidingExpiration = TimeSpan.FromSeconds(30),
                };
            }

            var strValue = JsonConvert.SerializeObject(value);
            var bytes = Encoding.UTF8.GetBytes(strValue);
            cache.Set(key, bytes, options);
        }
    }
}