using ClothingStoreManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateEmployeeDto
    {
        [Required(ErrorMessage = "اسم الموظف مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم طويل جداً")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "نوع الموظف مطلوب")]
        public EmployeeType? Type { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "رقم الهاتف غير صحيح")]
        public string? Phone { get; set; }

        [Range(0, 100000, ErrorMessage = "المرتب يجب أن يكون قيمة منطقية")]
        public decimal BaseSalary { get; set; } = 0;
    }
}
