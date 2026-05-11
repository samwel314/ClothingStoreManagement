namespace ClothingStoreManagement.Application.DTO
{
    public class PaymentSourceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
