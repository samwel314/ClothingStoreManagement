using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Application.DTO
{
    public class EmployeeTransactionsDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int Days { get; set; }   
        public EmployeeTransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}
