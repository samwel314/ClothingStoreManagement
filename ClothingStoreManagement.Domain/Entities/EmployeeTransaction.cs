namespace ClothingStoreManagement.Domain.Entities
{
    public class EmployeeTransaction
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public EmployeeTransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public int? DaysCount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!; 
    }
}
