
using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    public class PaymentSourceRepository : BaseRepository<PaymentSource>, IPaymentSourceRepository
    {
        public PaymentSourceRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
