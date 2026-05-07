using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    internal class StockMovementRepository : BaseRepository<StockMovement>, IStockMovementRepository
    {
        private readonly ApplicationDbContext _db;

        public StockMovementRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
