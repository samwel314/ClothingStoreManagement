using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Application.DTO
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; } = null!; 
        public InvoiceStatus Status { get; set;}
        public decimal TotalAmount { get; set; }    
        public DateTime LastUpdate { get; set; }
    }
}
