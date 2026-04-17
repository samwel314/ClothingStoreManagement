using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateUpdateColorDto
    {
        public int? Id { get; set; } // for update, null for create
        [MaxLength(50, ErrorMessage = "اقصي عدد حروف هو 50 حرف ")]
        [Required(ErrorMessage = "ادخل اسم اللون ")]
        public string Name { get; set; } = null!;
        [MaxLength(9, ErrorMessage = "أقصى عدد حروف لكود اللون هو 9 أحرف (بما في ذلك #)")]
        [MinLength(4, ErrorMessage = "أقل عدد حروف هو 4 (مثل #FFF)")]
        [Required(ErrorMessage = "ادخل كود اللون")]
        [RegularExpression("^#([A-Fa-f0-9]{3}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$",
        ErrorMessage = "صيغة كود اللون غير صحيحة (يجب أن يبدأ بـ #)")]
        public string Code { get; set; } = "#000000"; // قيمة افتراضية سوداء    }
    }
    public class ColorListDTO 
    {
        public int Id { get; set; } 
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!; 


    }
}

