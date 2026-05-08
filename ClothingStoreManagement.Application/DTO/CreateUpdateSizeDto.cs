using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateUpdateSizeDto
    {
        public int? Id { get; set; }
        [MaxLength(50, ErrorMessage = "أقصى عدد حروف هو 50 حرف")]
        [Required(ErrorMessage = "ادخل اسم المقاس")]
        public string Name { get; set; } = null!; // مثال: Large

        [MaxLength(10, ErrorMessage = "أقصى عدد حروف لكود المقاس هو 10 أحرف")]
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
