using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateUpdateSizeDto
    {
        public int? Id { get; set; }
        [MaxLength(50, ErrorMessage = "أقصى عدد حروف هو 50 حرف")]
        [Required(ErrorMessage = "ادخل اسم المقاس")]
        public string Name { get; set; } = null!; // مثال: Large
        [MaxLength(3, ErrorMessage = "أقصى عدد حروف لكود المقاس هو 3 أحرف")]
        [MinLength(1, ErrorMessage = "أقل عدد حروف لكود المقاس هو حرف")]
        [RegularExpression(
    "^[A-Za-z0-9]+$",
    ErrorMessage = "كود المقاس يجب أن يحتوي على حروف وأرقام فقط")]
        [Required(ErrorMessage = "ادخل كود المقاس")]

        public string Code { get; set; } = null!; // مثال: L
    }

    public class SizeListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
