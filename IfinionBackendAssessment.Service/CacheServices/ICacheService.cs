using IfinionBackendAssessment.Entity.Entities;

namespace IfinionBackendAssessment.Service.CacheService
{
    public interface ICacheService<T> where T : class
    {
        List<T> GetDataFromCacheAsyc(string CacheKey);
        void SaveToCache(List<T> data, string key);
    }
}
