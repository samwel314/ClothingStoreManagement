
using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    public class EmployeeTransactionRepository : BaseRepository<EmployeeTransaction>, IEmployeeTransactionRepository
    {
        private readonly ApplicationDbContext _db;
        public EmployeeTransactionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}

