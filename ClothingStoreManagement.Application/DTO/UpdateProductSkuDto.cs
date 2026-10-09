using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class UpdateProductSkuDto
    {
        public Guid ProductId { get; set; }
        [Required(ErrorMessage = "كود المنتج (SKU) مطلوب")]
        [StringLength(3, MinimumLength = 1, ErrorMessage = "كود المنتج يجب أن يكون بين 1 و 3 حروف")]
        [RegularExpression("^[A-Za-z0-9]+$$", ErrorMessage = "كود المنتج يجب أن يحتوي على حروف وأرقام فقط")]
        public string SKU { get; set; } = string.Empty;

    }
}

