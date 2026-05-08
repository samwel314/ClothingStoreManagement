namespace ClothingStoreManagement.Application.DTO
{
    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdate { get; set; }
        public int TotalSalesCount { get; set; }    
        public decimal NetProfit { get; set; }  
        public decimal TotalSalesAmount { get; set; }
        public TopVariantDTO ? TopVariant { get; set; } = null!; 
        public IEnumerable<ProductVariantListDto> Variants { get; set; } = new List<ProductVariantListDto>();
    }
}
