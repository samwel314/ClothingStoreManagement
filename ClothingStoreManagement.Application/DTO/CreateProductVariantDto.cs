using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateProductVariantDto
    {
        [Required(ErrorMessage = "المقاس مطلوب")]
        public int SizeId { get; set; }
        [Required(ErrorMessage = "اللون مطلوب")]
        public int ColorId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "الكمية لا يمكن أن تكون اقل من صفر ")]
        public int StockQuantity { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "سعر البيع يجب أن يكون أكبر من صفر")]
        public decimal SellingPrice { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "سعر الشراء يجب أن يكون أكبر من صفر")]
        public decimal PurchasePrice { get; set; }
    }
}
