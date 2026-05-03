using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class UpdateProductSkuDto
    {
        public Guid ProductId { get; set; }
        [Required(ErrorMessage = "كود المنتج (SKU) مطلوب")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "كود المنتج يجب أن يكون بين 4 و 100 حرف")]
        public string SKU { get; set; } = string.Empty;

    }
}

