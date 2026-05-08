using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreManagement.Data.Repository
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        void Detach(Invoice model); 
      
    }
}
