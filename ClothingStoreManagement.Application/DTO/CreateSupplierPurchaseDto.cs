namespace ClothingStoreManagement.Application.DTO
{
    public class CreateSupplierPurchaseDto
    {
        public int SupplierId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public string? Notes { get; set; }
    }
}
