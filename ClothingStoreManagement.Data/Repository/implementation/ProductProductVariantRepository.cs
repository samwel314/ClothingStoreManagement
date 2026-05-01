using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    public class ProductProductVariantRepository : BaseRepository<ProductVariant>, IProductProductVariantRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductProductVariantRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
