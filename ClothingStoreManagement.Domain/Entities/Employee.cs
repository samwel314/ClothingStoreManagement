using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الموظف مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم طويل جداً")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "نوع الموظف مطلوب")]
        public EmployeeType Type { get; set; }
        [StringLength(11, ErrorMessage = "رقم الهاتف يجب أن يكون 11 رقم")]
        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "رقم الهاتف غير صحيح")]
        public string? Phone { get; set; }
        public decimal BaseSalary { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public IEnumerable<ShiftTransaction> ShiftTransactions { get; set; } = new List<ShiftTransaction>();
        public IEnumerable<Invoice> Invoices { get; set; } = new List<Invoice>();
        public void ToggleStatus() => IsActive = !IsActive;

    }
}
