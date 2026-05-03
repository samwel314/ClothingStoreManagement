namespace ClothingStoreManagement.Application.DTO
{
    public class ProductVariantListDto
    {
        public Guid Id { get; set; }
        public string Size { get; set; } = null!;// code 
        public string Color { get; set; } = null!; //code 
        public decimal Price { get; set; } // بيع 
        public int ColorId { get; set; }    
        public int SizeId { get; set; } 
        public decimal Purchase { get; set; } 
        public int StockQuantity { get; set; }
        public string Sku { get; set; } = null!; 

    }
}
