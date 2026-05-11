using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateUpdatePaymentSourceDto
    {
        public int? Id { get; set; } // null في حالة الإضافة

        [Required(ErrorMessage = "يجب إدخال اسم وسيلة الدفع")]
        [StringLength(50, ErrorMessage = "الاسم طويل جداً")]
        public string Name { get; set; } = string.Empty;
    }
}
