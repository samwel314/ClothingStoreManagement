namespace ClothingStoreManagement.Application.DTO
{
    public class CreateProductWithVariantsDto
    {
        public string Name { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public int CategoryId { get; set; }
        public List<CreateProductVariantDto> Variants { get; set; } = new();
    }
}
