namespace ClothingStoreManagement.Application.DTO
{
    public class EmployeeDetails
    {
        public decimal Penalty {  get; set; }
        public decimal Absence  { get; set; }
        public decimal Bonus { get; set; }
        public decimal Borrow { get; set; }
        public int AbsenceDays { get; set; }  
        public decimal TotalSalary => Bonus + Borrow + Absence + Penalty;   
        public IEnumerable<EmployeeTransactionsDto> EmployeeTransactions { get; set; }  
    }
}
