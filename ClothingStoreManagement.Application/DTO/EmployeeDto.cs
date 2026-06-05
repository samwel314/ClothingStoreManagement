using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Application.DTO
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public EmployeeType Type { get; set; }
        public string? Phone { get; set; }
        public decimal BaseSalary { get; set; }
        public bool IsActive { get; set; }
    }
}
