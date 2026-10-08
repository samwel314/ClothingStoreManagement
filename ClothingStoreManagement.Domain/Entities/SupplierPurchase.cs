namespace ClothingStoreManagement.Domain.Entities
{
    public class SupplierPurchase
    {
        public int Id { get; set; }

        public int SupplierId { get; set; }

        public Supplier Supplier { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount =>
            TotalAmount - PaidAmount;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? Notes { get; set; }
    }
}
