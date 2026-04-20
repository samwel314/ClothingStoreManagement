using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateProductVariantDto
    {
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public int StockQuantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
