using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        private readonly ApplicationDbContext _db;
        public InvoiceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
