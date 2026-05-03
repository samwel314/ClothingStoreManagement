using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> _dbSet;
        public BaseRepository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();  
        }
        public async Task CreateAsync(T model)
        {
            await _dbSet.AddAsync(model);
        }
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AnyAsync(predicate);
        }

        public IQueryable<T> GetAll(bool track  = false)
        {
            if (track)
                return _dbSet; 
            return _dbSet.AsNoTracking();
        }
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool track = false)
        {
            if (track)
                 return await _dbSet.FirstOrDefaultAsync(predicate);

            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public void Delete(T model)
        {
            _db.Remove(model); 
        }
        public void Delete(IEnumerable <T> models)
        {
            _db.RemoveRange(models);
        }
    }
}
