using System.Linq.Expressions;

namespace ClothingStoreManagement.Data.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        Task CreateAsync(T model);
        void Delete(T model);
        void Delete(IEnumerable<T> models);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool track = false);
        IQueryable<T> GetAll(bool track = false);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
