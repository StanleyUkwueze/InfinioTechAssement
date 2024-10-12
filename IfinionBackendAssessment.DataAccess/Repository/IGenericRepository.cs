using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.DataAccess.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> SaveAsync();
        IQueryable<T> GetAll();
        T GetFirstOrDefauly(Expression<Func<T, bool>>? filter = null);
        Task<bool> RemoveAsync(T entity);
        Task<bool> RemoveRangeAsync(IEnumerable<T> entity);
    }
}
