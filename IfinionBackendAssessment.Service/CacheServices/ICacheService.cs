using IfinionBackendAssessment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.CacheService
{
    public interface ICacheService
    {
        List<Product> GetProductFromCacheAsyc(int itemId);
        List<Product> GetProductsFromCacheAsyc();
        void SaveToCache(List<Product> data, string key);
    }
}
