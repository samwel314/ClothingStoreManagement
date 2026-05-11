
using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    public class ShiftRepository : BaseRepository<Shift>, IShiftRepository  
    {
        private readonly ApplicationDbContext _db;
        public ShiftRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
