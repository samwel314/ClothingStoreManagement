namespace ClothingStoreManagement.Application.DTO
{
    public class PaymentTypeSummary
    {
        public string Name { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public bool IsCashSource { get; set; } 
    }
}
