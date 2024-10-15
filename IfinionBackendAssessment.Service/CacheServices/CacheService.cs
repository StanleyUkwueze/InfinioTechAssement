using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.CacheService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IfinionBackendAssessment.Service.CacheServices
{
    public class CacheService<T>(IConfiguration configuration, IMemoryCache cache, ILogger<CacheService<T>> logger) : ICacheService<T> where T : class
    {
        private static string CacheKey = string.Empty;

        public void SaveToCache(List<T> data, string key)
        {
            var expirationTime = int.Parse(configuration.GetSection("CacheSettings:ExpirationTime").Value!);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expirationTime),
                SlidingExpiration = TimeSpan.FromHours(expirationTime),
                Priority = CacheItemPriority.Normal
            };

            cache.Set(key, data, cacheEntryOptions);
        }

        public List<T> GetDataFromCacheAsyc(string CacheKey)
        {
            if (!cache.TryGetValue(CacheKey, out List<T>? data))
            {
                logger.LogInformation($"No item found in the Cache");
                return data!;
            }
            return data!.ToList();
        }
    }
}
