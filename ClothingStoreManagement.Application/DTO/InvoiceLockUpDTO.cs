using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Application.DTO
{
    public class InvoiceLockUpDTO
    {
        public int Id { get; set; }
        public string Serial { get; set; } = null!;
        public InvoiceStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime LastUpdatedAt { get;  set; } 
    }
}
