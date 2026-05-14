using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    //
    public class InvoicePaymentRepository : BaseRepository<InvoicePayment>, IInvoicePaymentRepository
    {
        private readonly ApplicationDbContext _db;

        public InvoicePaymentRepository (ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
