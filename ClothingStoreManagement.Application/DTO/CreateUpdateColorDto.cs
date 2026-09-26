using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateUpdateColorDto
    {
        public int? Id { get; set; } // for update, null for create

        [Required(ErrorMessage = "ادخل اسم اللون")]
        [MaxLength(50, ErrorMessage = "أقصى عدد حروف هو 50 حرف")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "ادخل كود اللون")]
        [MaxLength(10, ErrorMessage = "أقصى عدد حروف لكود اللون هو 10 أحرف")]
        [MinLength(2, ErrorMessage = "أقل عدد حروف لكود اللون هو حرفين")]
        [RegularExpression(
            "^[A-Za-z0-9]+$",
            ErrorMessage = "كود اللون يجب أن يحتوي على حروف وأرقام فقط")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "ادخل قيمة اللون")]
        [RegularExpression(
            "^#([A-Fa-f0-9]{3}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$",
            ErrorMessage = "صيغة قيمة اللون غير صحيحة (مثل #FFF أو #000000)")]
        public string HexCode { get; set; } = "#000000";
    }
    public class ColorListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;

        public string HexCode { get; set; } = null!;

    }
}

