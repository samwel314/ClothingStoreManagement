namespace ClothingStoreManagement.Application.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
    public class SupplierDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? AdditionalPhone { get; set; }

        public string? Address { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal TotalPurchases { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal Remaining { get; set; }
    }

    public class SupplierPurchaseDto
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Notes { get; set; }
    }

    public class CreateSupplierPaymentDto
    {
        public int SupplierId { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
    }
}
