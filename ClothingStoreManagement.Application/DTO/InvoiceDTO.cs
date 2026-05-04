using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Application.DTO
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; } = null!;
        public InvoiceStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountWithDiscount { get; set; }
        public DateTime LastUpdate { get; set; }
        public int TotalQuantity { get; set; }

        public void UpdateTotal()
        {
            TotalAmount = 0;
            foreach (var item in Items)
            {
                TotalAmount += item.Quantity * item.Price;
            }
        }
        public void UpdateTotalQuantity()
        {
            TotalQuantity = 0;
            foreach (var item in Items)
            {
                TotalQuantity += item.Quantity;
            }
        }
        public void UpdateTotalWithDiscount()
        {
            TotalAmountWithDiscount = 0;
            foreach (var item in Items)
            {
                TotalAmountWithDiscount += item.Total;
            }
        }
        public List<InvoiceItemDetailsDto> Items { get; set; } = new List<InvoiceItemDetailsDto>();
    }
}
