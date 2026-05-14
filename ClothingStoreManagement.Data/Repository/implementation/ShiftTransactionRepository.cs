
using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    public class ShiftTransactionRepository : BaseRepository<ShiftTransaction>, IShiftTransactionRepository
    {
        private readonly ApplicationDbContext _db;
        public ShiftTransactionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
