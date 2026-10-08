namespace ClothingStoreManagement.Application.DTO
{
    public class UpdateSupplierDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? AdditionalPhone { get; set; }

        public string? Address { get; set; }

        public string? Notes { get; set; }
    }
}
