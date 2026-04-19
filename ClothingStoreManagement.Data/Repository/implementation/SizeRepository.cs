using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    public class SizeRepository : BaseRepository<Size>, ISizeRepository
    {
        private readonly ApplicationDbContext _db;

        public SizeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
