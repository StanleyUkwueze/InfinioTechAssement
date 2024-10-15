using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.CacheService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IfinionBackendAssessment.Service.CacheServices
{
    public class CacheService(IConfiguration configuration, IMemoryCache cache, ILogger<CacheService> logger) : ICacheService
    {
        private static string CacheKey = string.Empty;

        public void SaveToCache(List<Product> data, string key)
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
        public List<Product> GetProductFromCacheAsyc(int itemId)
        {
            CacheKey = $"{configuration.GetSection("CacheSettings:CacheKey").Value!}_{itemId}";
            logger.LogInformation($"About to fetch products for cache with key: {CacheKey}");

            if (!cache.TryGetValue(CacheKey, out List<Product>? data))
            {
                logger.LogInformation($"No product found in the Cache");
                return data!;
            }
            return data!;
        }

        public List<Product> GetProductsFromCacheAsyc()
        {
            CacheKey = $"{configuration.GetSection("CacheSettings:CacheKey").Value!}";
            logger.LogInformation($"About to fetch products for cache with key: {CacheKey}");

            if (!cache.TryGetValue(CacheKey, out List<Product>? data))
            {
                logger.LogInformation($"No product found in the Cache");
                return data!;
            }
            return data!.ToList();
        }
    }
}
