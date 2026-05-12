namespace ClothingStoreManagement.Application.DTO
{
    public class PaymentPartDto
    {
        public int PaymentSourceId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Amount { get; set; } = 0; // القيمة الافتراضية صفر
        public bool IsCash { get; set; }
    }
}
