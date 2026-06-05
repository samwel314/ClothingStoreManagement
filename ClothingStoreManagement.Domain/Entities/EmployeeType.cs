using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Domain.Entities
{
    public enum EmployeeType
    {
        [Display(Name = "مدير المحل")]
        Manager = 1,
        [Display(Name = "موظف مبيعات / صالة")]
        SalesStaff = 2,
        [Display(Name = "طيار / دليفري")]
        Delivery = 3,
        [Display(Name = "عامل / بوفيه / خدمات")]
        Worker = 4,
        [Display(Name = "كاشير")]
        Cashier = 5 
    }
}
