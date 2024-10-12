using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.DataAccess.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        internal DbSet<T> dbSet;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }
        public async Task<bool> AddAsync(T entity)
        {
            dbSet.Add(entity);
            return await SaveAsync();
        }

        public IQueryable<T> GetAll()
        {
            IQueryable<T> query = dbSet.AsQueryable();
            return query;
        }


        public T GetFirstOrDefauly(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.FirstOrDefault();
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            return await SaveAsync();
        }

        public async Task<bool> RemoveRangeAsync(IEnumerable<T> entity)
        {
            dbSet.RemoveRange();
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            return await SaveAsync();
        }
    }
}
