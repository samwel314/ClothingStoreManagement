namespace ClothingStoreManagement.Application.DTO
{
    public class DisplayPaymentPartDto
    {
        public int PaymentSourceId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Amount { get; set; } = 0; // القيمة الافتراضية صفر
    }
}
