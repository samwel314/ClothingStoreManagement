using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class UpdateProductBasicDto
    {
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم المنتج يجب أن يكون بين 2 و 100 حرف")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = " اختار الفئة ")]
        public int CategoryId { get; set; }
    }
}

