
using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    public class MainTreasuryTransactionRepository : BaseRepository<MainTreasuryTransaction>, IMainTreasuryTransactionRepository
    {
        private readonly ApplicationDbContext _db;
        public MainTreasuryTransactionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
