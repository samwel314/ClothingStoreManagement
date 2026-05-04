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
    public class InvoiceItemDetailsDto
    {
        public string ProductName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Total  => (Price * Quantity) * (1 - Discount / 100 );    
    }
}
