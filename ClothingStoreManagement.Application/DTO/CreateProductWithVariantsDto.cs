using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateProductWithVariantsDto
    {
        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم المنتج يجب أن يكون بين 2 و 100 حرف")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "كود المنتج (SKU) مطلوب")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "كود المنتج يجب أن يكون بين 4 و 100 حرف")]
        public string SKU { get; set; } = null!;
        [Required(ErrorMessage = "يجب اختيار القسم")]
        [Range(1, int.MaxValue, ErrorMessage = "القسم المختار غير صحيح")]
        public int CategoryId { get; set; }
        public List<CreateProductVariantDto> Variants { get; set; } = new ();
    }
}
